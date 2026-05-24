using Almoxarifado.Application.Queries;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using MediatR;
namespace Almoxarifado.Application.Handlers;
public sealed class GetSolicitacoesByUserHandler(IReadOnlyRepository<Solicitacao> repository)
    : IRequestHandler<GetSolicitacoesByUserQuery, IEnumerable<Solicitacao>>
{
    public async Task<IEnumerable<Solicitacao>> Handle(GetSolicitacoesByUserQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByUserIdAsync(request.UsuarioId);
    }
}
