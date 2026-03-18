using Supabase;
using Almoxarifado.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Supabase
var supabaseUrl = builder.Configuration["Supabase:Url"]!;
var supabaseKey = builder.Configuration["Supabase:Key"]!;

builder.Services.AddScoped(_ =>
    new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true,
    }));


builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
//builder.Services.AddSingleton<PedidoService>(); -- ele serve para a criação de instancias dessa classe seja individual e caso alguem queira criar uma instancia nova, será usado uma instancia já criada, ou seja, a mesma instancia é usada para toda a aplicação.

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();