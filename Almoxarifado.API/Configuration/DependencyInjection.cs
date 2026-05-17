using Firebase.Database;
using Almoxarifado.API.Repositories;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Handlers;
using Almoxarifado.Application.Behaviors;
using Almoxarifado.API.Middleware;
using FluentValidation;
using MediatR;

namespace Almoxarifado.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationIdHeaderHandler>();

        var firebaseUrl = configuration["Firebase:Url"]!;
        var firebaseSecret = configuration["Firebase:Secret"]!;

        var firebaseClient = new FirebaseClient(firebaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(firebaseSecret)
        });

        services.AddSingleton(firebaseClient);

        services.AddScoped<IEngine<Estoque>>(sp => new FirebaseEngine<Estoque>(firebaseClient, "estoque"));
        services.AddScoped<IEngine<Solicitacao>>(sp => new FirebaseEngine<Solicitacao>(firebaseClient, "solicitacoes"));

        services.AddScoped<IReadOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());
        services.AddScoped<IWriteOnlyRepository<Estoque>>(sp => sp.GetRequiredService<IEngine<Estoque>>());

        services.AddScoped<IReadOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());
        services.AddScoped<IWriteOnlyRepository<Solicitacao>>(sp => sp.GetRequiredService<IEngine<Solicitacao>>());

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(CreateSolicitacaoHandler).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }
}
