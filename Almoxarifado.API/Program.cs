using Almoxarifado.API.Configuration;
using Almoxarifado.API.Middleware;
using Almoxarifado.Application;
using Almoxarifado.Infrastructure;
using Almoxarifado.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options => { options.AddServerHeader = false; options.ListenAnyIP(5003); });

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var projectId = builder.Configuration["Firebase:ProjectId"]
                ?? throw new ArgumentNullException("Firebase ProjectId não configurado no appsettings.");

var credentialsPath = Path.Combine(AppContext.BaseDirectory, builder.Configuration["Firebase:CredentialsPath"]!);
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileAppPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                if (claimsIdentity is null) return;

                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? claimsIdentity.FindFirst("user_id")?.Value;

                if (string.IsNullOrEmpty(userId)) return;

                if (!claimsIdentity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
                }

                
                var firestoreDb = context.HttpContext.RequestServices.GetRequiredService<FirestoreDb>();
                var userDoc = await firestoreDb.Collection("usuarios").Document(userId).GetSnapshotAsync();

                if (userDoc.Exists && userDoc.ContainsField("tipo"))
                {
                    var tipoUsuario = userDoc.GetValue<int>("tipo");
                    if (tipoUsuario == 2)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Almoxarife"));
                    }
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AlmoxarifeOnly", policy => policy.RequireRole("Almoxarife"));
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("MobileAppPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));
app.MapControllers();

app.Run();

public partial class Program { }