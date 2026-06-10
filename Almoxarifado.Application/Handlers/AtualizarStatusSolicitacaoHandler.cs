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
using System.Linq;
using Google.Cloud.Firestore;

namespace Almoxarifado.Application.Handlers;

public sealed class AtualizarStatusSolicitacaoHandler(
    IReadOnlyRepository<Solicitacao> readRepository,
    IReadOnlyRepository<Produto> produtoReadRepo,
    FirestoreDb firestoreDb)
    : IRequestHandler<AtualizarStatusSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> Handle(AtualizarStatusSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        var solicitacao = await readRepository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Solicitação '{request.Id}' não encontrada.");

        if (request.NovoStatus == StatusSolicitacao.Aprovada && solicitacao.Status != StatusSolicitacao.Aprovada)
        {
            var refsProdutos = new Dictionary<string, DocumentReference>();

            foreach (var item in solicitacao.Itens)
            {
                var produtosResult = await produtoReadRepo.GetByFieldAsync("NumCode", item.ProdutoId);

                var produtoEncontrado = produtosResult.FirstOrDefault()
                    ?? throw new KeyNotFoundException($"Produto SKU '{item.ProdutoId}' não encontrado no banco.");

                refsProdutos.Add(item.Id, firestoreDb.Collection("produtos").Document(produtoEncontrado.Id));
            }

            var solicitacaoRef = firestoreDb.Collection("solicitacoes").Document(solicitacao.Id);

            await firestoreDb.RunTransactionAsync(async transaction =>
            {
                var solicitacaoSnapshot = await transaction.GetSnapshotAsync(solicitacaoRef);
                var statusAtualBanco = solicitacaoSnapshot.GetValue<string>("Status");

                if (statusAtualBanco != StatusSolicitacao.Pendente)
                    throw new InvalidOperationException("Esta solicitação já foi processada por outro operador.");

                var atualizacoesEstoque = new Dictionary<DocumentReference, long>();

                foreach (var item in solicitacao.Itens)
                {
                    var prodRef = refsProdutos[item.Id];
                    var snapshot = await transaction.GetSnapshotAsync(prodRef);

                    var estoqueAtual = snapshot.GetValue<long>("qtdEstoque");

                    if (estoqueAtual < item.Quantidade)
                    {
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para SKU '{item.ProdutoId}'. Solicitado: {item.Quantidade}, Disponível: {estoqueAtual}.");
                    }

                    atualizacoesEstoque.Add(prodRef, estoqueAtual - item.Quantidade);
                }

                foreach (var (prodRef, novoEstoque) in atualizacoesEstoque)
                {
                    transaction.Update(prodRef, "qtdEstoque", novoEstoque);
                    transaction.Update(prodRef, "updatedAt", DateTime.UtcNow);
                }

                transaction.Update(solicitacaoRef, "Status", StatusSolicitacao.Aprovada);
                transaction.Update(solicitacaoRef, "UpdatedAt", DateTime.UtcNow);
            });

            solicitacao.Aprovar();

            return solicitacao.ToDto();
        }

        Action acao = request.NovoStatus switch
        {
            StatusSolicitacao.Aprovada => solicitacao.Aprovar,
            StatusSolicitacao.Recusada => solicitacao.Recusar,
            StatusSolicitacao.Entregue => solicitacao.FinalizarEntrega,
            StatusSolicitacao.Cancelada => solicitacao.Cancelar,
            _ => throw new ArgumentException($"Status '{request.NovoStatus}' inválido.")
        };

        acao();

        var solRef = firestoreDb.Collection("solicitacoes").Document(solicitacao.Id);

        await solRef.UpdateAsync(new Dictionary<string, object>
        {
            { "Status", solicitacao.Status },
            { "UpdatedAt", DateTime.UtcNow }
        });

        return solicitacao.ToDto();
    }
}