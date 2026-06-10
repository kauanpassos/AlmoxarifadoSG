using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Constants;
using MediatR;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Almoxarifado.Application.Handlers;

public sealed class AtualizarStatusSolicitacaoHandler(
    IReadOnlyRepository<Solicitacao> readRepository,
    IWriteOnlyRepository<Solicitacao> writeRepository,
    IEstoqueTransactionService estoqueTransactionService)
    : IRequestHandler<AtualizarStatusSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> Handle(AtualizarStatusSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        var solicitacao = await readRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Solicitação '{request.Id}' não encontrada.");

        if (request.NovoStatus == StatusSolicitacao.Aprovada)
        {
            var solicitacaoAprovada = await estoqueTransactionService.ProcessarAprovacaoSolicitacaoAsync(request.Id);
            return solicitacaoAprovada.ToDto();
        }

        Action acao = request.NovoStatus switch
        {
            StatusSolicitacao.Recusada => solicitacao.Recusar,
            StatusSolicitacao.Entregue => solicitacao.FinalizarEntrega,
            StatusSolicitacao.Cancelada => solicitacao.Cancelar,
            _ => throw new ArgumentException($"Status '{request.NovoStatus}' inválido.")
        };

        acao();

        await writeRepository.UpdateAsync(solicitacao);

        return solicitacao.ToDto();
    }
}