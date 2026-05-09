using Firebase.Database;
using Almoxarifado.API.Repositories;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Handlers;
using Almoxarifado.Application.Behaviors;
using FluentValidation;
using MediatR;

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

        // Registro dos motores genéricos (FirebaseEngine).
        services.AddScoped<IEngine<Estoque>>(sp => new FirebaseEngine<Estoque>(firebaseClient, "estoque"));
        services.AddScoped<IEngine<Solicitacao>>(sp => new FirebaseEngine<Solicitacao>(firebaseClient, "solicitacoes"));

        // Interfaces segregadas (ISP) para Estoque.
        services.AddScoped<IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        services.AddScoped<IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        
        // Interfaces segregadas (ISP) para Solicitação.
        services.AddScoped<IReadOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());
        services.AddScoped<IWriteOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Usamos um Handler como referência para localizar o Assembly da camada de Application.
        var assembly = typeof(CreateSolicitacaoHandler).Assembly;

        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            // Injeção do pipeline de validação automática.
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Registro automático de todos os validadores FluentValidation.
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
