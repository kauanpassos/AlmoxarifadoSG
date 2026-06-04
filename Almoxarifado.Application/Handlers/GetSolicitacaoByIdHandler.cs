using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using Almoxarifado.Application.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace Almoxarifado.Application.Handlers;

public sealed class GetSolicitacaoByIdHandler(IReadOnlyRepository<Solicitacao> repository) 
    : IRequestHandler<GetSolicitacaoByIdQuery, SolicitacaoDto?>
{
    public async Task<SolicitacaoDto?> Handle(GetSolicitacaoByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetByIdAsync(request.Id);
        return result?.ToDto();
    }
}
