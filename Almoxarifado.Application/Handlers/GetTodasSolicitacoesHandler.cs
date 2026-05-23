using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;
namespace Almoxarifado.Application.Handlers;
public sealed class GetTodasSolicitacoesHandler(IReadOnlyRepository<Solicitacao> repository)
    : IRequestHandler<GetTodasSolicitacoesQuery, IEnumerable<Solicitacao>>
{
    public async Task<IEnumerable<Solicitacao>> Handle(GetTodasSolicitacoesQuery request, CancellationToken cancellationToken)
        => await repository.GetAllAsync();
}
