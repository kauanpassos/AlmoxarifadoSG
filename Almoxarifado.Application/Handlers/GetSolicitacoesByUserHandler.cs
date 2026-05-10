using Almoxarifado.Application.Queries;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using MediatR;

namespace Almoxarifado.Application.Handlers;

// Handler para consulta de solicitações filtrada por usuário.
public sealed class GetSolicitacoesByUserHandler(IReadOnlyRepository<Solicitacao> repository)
    : IRequestHandler<GetSolicitacoesByUserQuery, IEnumerable<Solicitacao>>
{
    public async Task<IEnumerable<Solicitacao>> Handle(GetSolicitacoesByUserQuery request, CancellationToken cancellationToken)
    {
        // Utilizamos a extensão especializada de busca por usuário do domínio.
        return await repository.GetByUserIdAsync(request.UsuarioId);
    }
}
