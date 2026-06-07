using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Almoxarifado.Application.Handlers;

public sealed class AtualizarStatusSolicitacaoHandler(
    IReadOnlyRepository<Solicitacao> readRepository,
    IWriteOnlyRepository<Solicitacao> writeRepository,
    IReadOnlyRepository<Produto> produtoReadRepo,
    IWriteOnlyRepository<Produto> produtoWriteRepo)
    : IRequestHandler<AtualizarStatusSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> Handle(AtualizarStatusSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        var solicitacao = await readRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Solicitação '{request.Id}' não encontrada.");

        // Regra de Ouro: Só baixa estoque se a nova ação for "Aprovada" e se ela já não estava aprovada antes
        if (request.NovoStatus == "Aprovada" && solicitacao.Status != "Aprovada")
        {
            var produtosParaAtualizar = new List<(Produto Produto, long Quantidade)>();

            // 1. FASE DE VALIDAÇÃO (Fail Fast)
            foreach (var item in solicitacao.Itens)
            {
                var idDoProduto = item.ProdutoId;

                var produto = await produtoReadRepo.GetByIdAsync(idDoProduto)
                    ?? throw new KeyNotFoundException($"Produto com ID '{idDoProduto}' não encontrado no banco de dados.");

                if (produto.QtdEstoque < item.Quantidade)
                {
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para a peça '{produto.Nome}'. Solicitado: {item.Quantidade}, Disponível: {produto.QtdEstoque}.");
                }

                produtosParaAtualizar.Add((produto, (long)item.Quantidade));
            }

            // 2. FASE DE EFETIVAÇÃO
            foreach (var (produto, quantidade) in produtosParaAtualizar)
            {
                produto.BaixarEstoque(quantidade);
                await produtoWriteRepo.UpdateAsync(produto);
            }
        }

        // 🔥 CORREÇÃO AQUI: Adicionada a opção "Cancelado" para mapear com a Entidade
        Action acao = request.NovoStatus switch
        {
            "Aprovada" => solicitacao.Aprovar,
            "Recusada" => solicitacao.Recusar,
            "Entregue" => solicitacao.FinalizarEntrega,
            "Cancelado" => solicitacao.Cancelar, // <-- AGORA ELE RECONHECE O CANCELAMENTO!
            _ => throw new ArgumentException($"Status '{request.NovoStatus}' inválido. Use: Aprovada, Recusada, Entregue ou Cancelado.")
        };

        acao();

        // Salva a solicitação com o novo status no banco
        await writeRepository.UpdateAsync(solicitacao);

        return solicitacao.ToDto();
    }
}