using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Almoxarifado.API;
using Almoxarifado.Domain.Exceptions;

namespace TesteApp.IntegrationTests;

public class ExceptionMiddlewareIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ExceptionMiddlewareIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ShouldReturn422_WhenUnprocessableEntityExceptionIsThrown()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddRouting();
            });
            builder.Configure(app =>
            {
                app.UseMiddleware<Almoxarifado.API.Middleware.ExceptionMiddleware>();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test-422", () => {
                        throw new UnprocessableEntityException("Regra de negócio violada.");
                    });
                });
            });
        }).CreateClient();

        var response = await client.GetAsync("/test-422");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("Requisição Não Processável", problemDetails.Title);
        Assert.Equal("Regra de negócio violada.", problemDetails.Detail);
        Assert.True(problemDetails.Extensions.ContainsKey("traceId"));
    }

    [Fact]
    public async Task Get_ShouldReturn409_WhenConflictExceptionIsThrown()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddRouting(); });
            builder.Configure(app =>
            {
                app.UseMiddleware<Almoxarifado.API.Middleware.ExceptionMiddleware>();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test-409", () => {
                        throw new ConflictException("Usuário já existe.");
                    });
                });
            });
        }).CreateClient();

        var response = await client.GetAsync("/test-409");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("Conflito de Dados", problemDetails.Title);
        Assert.Equal("Usuário já existe.", problemDetails.Detail);
    }

    [Fact]
    public async Task Get_ShouldReturn400_WhenGenericDomainExceptionIsThrown_EnsuringBackwardCompatibility()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services => { services.AddRouting(); });
            builder.Configure(app =>
            {
                app.UseMiddleware<Almoxarifado.API.Middleware.ExceptionMiddleware>();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test-400", () => {
                        throw new EstoqueInsuficienteException("Parafuso", 10, 5);
                    });
                });
            });
        }).CreateClient();

        var response = await client.GetAsync("/test-400");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal("Violação de Regra de Negócio", problemDetails.Title);
        Assert.Contains("Estoque insuficiente para a peça 'Parafuso'", problemDetails.Detail);
    }
}
