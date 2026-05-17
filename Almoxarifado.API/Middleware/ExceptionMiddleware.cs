using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FluentValidation;
using Almoxarifado.Domain.Exceptions;
using Almoxarifado.API.Configuration;

namespace Almoxarifado.API.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha capturada no pipeline da API.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var statusCode = exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ConflictException => HttpStatusCode.Conflict,
            UnprocessableEntityException => HttpStatusCode.UnprocessableEntity,
            NotFoundException => HttpStatusCode.NotFound,
            ForbiddenAccessException => HttpStatusCode.Forbidden,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            DomainException => HttpStatusCode.BadRequest, // Mantém compatibilidade reversa com o código legado
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var problem = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = GetTitle(exception),
            Detail = env.IsDevelopment() ? exception.Message : GetUserMessage(exception),
            Instance = context.Request.Path
        };

        var traceId = context.Items[HttpConstants.CorrelationIdHeader]?.ToString() ?? context.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;

        if (exception is ValidationException valEx)
        {
            problem.Extensions["errors"] = valEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        if (env.IsDevelopment())
        {
            problem.Extensions["stackTrace"] = exception.StackTrace;
        }

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = env.IsDevelopment()
        });

        await context.Response.WriteAsync(json);
    }

    private static string GetTitle(Exception ex) => ex switch
    {
        ValidationException => "Erro de Validação",
        ConflictException => "Conflito de Dados",
        UnprocessableEntityException => "Requisição Não Processável",
        NotFoundException => "Recurso Não Encontrado",
        ForbiddenAccessException => "Acesso Proibido",
        DomainException => "Violação de Regra de Negócio",
        _ => "Erro Interno do Servidor"
    };

    private static string GetUserMessage(Exception ex) => ex switch
    {
        ValidationException => "Um ou mais campos estão inválidos. Verifique os detalhes.",
        DomainException => ex.Message, // Captura todas as heranças de DomainException (Conflict, Unprocessable, etc)
        _ => "Ocorreu um erro inesperado. Tente novamente mais tarde."
    };
}
