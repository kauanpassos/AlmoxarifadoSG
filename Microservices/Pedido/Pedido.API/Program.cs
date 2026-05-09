using Microsoft.EntityFrameworkCore;
using Pedido.Infrastructure.Data;
using Pedido.Domain.Interfaces;
using Pedido.Infrastructure.Repositories;
using Pedido.Application.Interfaces;
using Pedido.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PedidoDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Pedido.Application.Handlers.CreateOrderHandler).Assembly));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();


var estoqueUrl = builder.Configuration["ExternalServices:EstoqueUrl"] ?? "http://estoque.api";
builder.Services.AddHttpClient<IEstoqueService, EstoqueApiClient>(client =>
{
    client.BaseAddress = new Uri(estoqueUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
