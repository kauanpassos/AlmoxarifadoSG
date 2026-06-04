using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Almoxarifado.Application.Handlers;

public sealed class AtualizarStatusSolicitacaoHandler(
    IReadOnlyRepository<Solicitacao> readRepository,
    IWriteOnlyRepository<Solicitacao> writeRepository)
    : IRequestHandler<AtualizarStatusSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> Handle(AtualizarStatusSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        var solicitacao = await readRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Solicitação '{request.Id}' não encontrada.");

        Action acao = request.NovoStatus switch
        {
            "Aprovada" => solicitacao.Aprovar,
            "Recusada" => solicitacao.Recusar,
            "Entregue" => solicitacao.FinalizarEntrega,
            _ => throw new ArgumentException($"Status '{request.NovoStatus}' inválido. Use: Aprovada, Recusada ou Entregue.")
        };

        acao();

        await writeRepository.UpdateAsync(solicitacao);
        
        return solicitacao.ToDto();
    }
}
