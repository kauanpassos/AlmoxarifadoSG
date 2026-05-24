using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FluentValidation;
using Almoxarifado.Domain.Exceptions;

namespace Almoxarifado.API.Middleware;
public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha capturada no pipeline da API.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            DomainException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var problem = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = GetTitle(exception),
            Detail = _env.IsDevelopment() ? exception.Message : GetUserMessage(exception),
            Instance = context.Request.Path
        };

        if (exception is ValidationException valEx)
        {
            problem.Extensions["errors"] = valEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        if (_env.IsDevelopment())
        {
            problem.Extensions["stackTrace"] = exception.StackTrace;
        }

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment()
        });

        await context.Response.WriteAsync(json);
    }

    private static string GetTitle(Exception ex) => ex switch
    {
        ValidationException => "Erro de Validação",
        DomainException => "Violação de Regra de Negócio",
        _ => "Erro Interno do Servidor"
    };

    private static string GetUserMessage(Exception ex) => ex switch
    {
        ValidationException => "Um ou mais campos estão inválidos. Verifique os detalhes.",
        DomainException => ex.Message,
        _ => "Ocorreu um erro inesperado. Tente novamente mais tarde."
    };
}
