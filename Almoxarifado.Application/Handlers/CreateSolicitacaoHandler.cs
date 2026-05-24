using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.Application.Handlers;
public sealed class CreateSolicitacaoHandler(IWriteOnlyRepository<Solicitacao> repository) 
    : IRequestHandler<CreateSolicitacaoCommand, Solicitacao>
{
    public async Task<Solicitacao> Handle(CreateSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        await repository.AddAsync(request.Solicitacao);
        return request.Solicitacao;
    }
}
