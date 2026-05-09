using Almoxarifado.API.Configuration;
using Almoxarifado.API.Middleware;

// O ponto de partida da nossa API. Aqui configuramos serviços e o pipeline de execução.
var builder = WebApplication.CreateBuilder(args);

// Registro de dependências do projeto. 
// Separamos em métodos de extensão para não poluir este arquivo principal.
builder.Services
    .AddInfrastructure(builder.Configuration) // Configura Banco, Repositórios e Firebase.
    .AddApplication();                        // Configura Serviços e Casos de Uso.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuração de CORS para permitir integração com o Mobile e Web.
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileAppPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ATENÇÃO: A ordem dos middlewares importa muito!
// Nosso ExceptionMiddleware deve ser um dos primeiros para capturar erros em qualquer camada abaixo.
app.UseMiddleware<ExceptionMiddleware>();

// Habilita o Swagger/OpenApi apenas em ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("MobileAppPolicy");
app.UseAuthorization();
app.MapControllers();

// Sobe o servidor e fica "escutando" as requisições.
app.Run();