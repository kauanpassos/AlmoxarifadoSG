using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;

namespace Almoxarifado.Application.Handlers;

// Handler responsável pela criação de solicitações.
// Refatorado para usar 'Solicitacao' em vez de 'Pedido'.
public sealed class CreateSolicitacaoHandler(IWriteOnlyRepository<Solicitacao> repository) 
    : IRequestHandler<CreateSolicitacaoCommand, Solicitacao>
{
    public async Task<Solicitacao> Handle(CreateSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        await repository.AddAsync(request.Solicitacao);
        return request.Solicitacao;
    }
}
