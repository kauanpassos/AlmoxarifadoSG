using FluentValidation;
using MediatR;

namespace Almoxarifado.Application.Behaviors;

// Pipeline Behavior que injeta validação automática em todo o fluxo do MediatR.
// Segue o padrão de Decorator para validar a requisição antes de processar o Handler.
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Executa todos os validadores registrados para este tipo de request em paralelo.
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            // Coleta todas as falhas encontradas.
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        // Se não houver erros, segue para o próximo passo (ou para o Handler final).
        return await next();
    }
}
