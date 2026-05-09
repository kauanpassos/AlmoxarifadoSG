using Firebase.Database;
using Almoxarifado.API.Repositories;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Handlers;
using Almoxarifado.Application.Behaviors;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace Almoxarifado.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var firebaseUrl = configuration["Firebase:Url"]!;
        var firebaseSecret = configuration["Firebase:Secret"]!;

        var firebaseClient = new FirebaseClient(firebaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(firebaseSecret)
        });

        services.AddSingleton(firebaseClient);

        services.AddScoped<IEngine<Estoque>>(sp => new FirebaseEngine<Estoque>(firebaseClient, "estoque"));
        services.AddScoped<IEngine<Pedido>>(sp => new FirebaseEngine<Pedido>(firebaseClient, "pedidos"));

        services.AddScoped<IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        services.AddScoped<IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        
        services.AddScoped<IReadOnlyRepository<Pedido>>(sp => sp.GetRequiredService<IEngine<Pedido>>());
        services.AddScoped<IWriteOnlyRepository<Pedido>>(sp => sp.GetRequiredService<IEngine<Pedido>>());

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(CreatePedidoHandler).Assembly;

        // Registro do MediatR com o pipeline de validação automática.
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            // Adicionamos o interceptor de validação. 
            // Ele rodará ANTES de qualquer Handler ser executado.
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Escaneia e registra automaticamente todos os validadores do FluentValidation no mesmo Assembly.
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
