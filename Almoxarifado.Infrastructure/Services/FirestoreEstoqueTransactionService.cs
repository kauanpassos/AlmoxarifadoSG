using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Almoxarifado.Domain.Constants;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Interfaces;
using Google.Cloud.Firestore;

namespace Almoxarifado.Infrastructure.Services;

public sealed class FirestoreEstoqueTransactionService : IEstoqueTransactionService
{
    private readonly FirestoreDb _firestoreDb;
    private readonly IReadOnlyRepository<Solicitacao> _solicitacaoReadRepo;
    private readonly IReadOnlyRepository<Produto> _produtoReadRepo;

    public FirestoreEstoqueTransactionService(
        FirestoreDb firestoreDb,
        IReadOnlyRepository<Solicitacao> solicitacaoReadRepo,
        IReadOnlyRepository<Produto> produtoReadRepo)
    {
        _firestoreDb = firestoreDb;
        _solicitacaoReadRepo = solicitacaoReadRepo;
        _produtoReadRepo = produtoReadRepo;
    }

    public async Task<Solicitacao> ProcessarAprovacaoSolicitacaoAsync(string solicitacaoId)
    {
        var solicitacao = await _solicitacaoReadRepo.GetByIdAsync(solicitacaoId)
            ?? throw new KeyNotFoundException($"Solicitação '{solicitacaoId}' não encontrada.");

        if (solicitacao.Status == StatusSolicitacao.Aprovada)
            return solicitacao;

        var refsProdutos = new Dictionary<string, DocumentReference>();

        foreach (var item in solicitacao.Itens)
        {
            var produtosResult = await _produtoReadRepo.GetByFieldAsync("NumCode", item.ProdutoId);
            var produtoEncontrado = produtosResult.FirstOrDefault()
                ?? throw new KeyNotFoundException($"Produto SKU '{item.ProdutoId}' não encontrado no banco.");

            refsProdutos.Add(item.Id, _firestoreDb.Collection("produtos").Document(produtoEncontrado.Id));
        }

        var solicitacaoRef = _firestoreDb.Collection("solicitacoes").Document(solicitacao.Id);

        await _firestoreDb.RunTransactionAsync(async transaction =>
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

                var estoqueAtual = snapshot.GetValue<long>("QtdEstoque");

                if (estoqueAtual < item.Quantidade)
                {
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para SKU '{item.ProdutoId}'. Solicitado: {item.Quantidade}, Disponível: {estoqueAtual}.");
                }

                atualizacoesEstoque.Add(prodRef, estoqueAtual - item.Quantidade);
            }

            foreach (var (prodRef, novoEstoque) in atualizacoesEstoque)
            {
                transaction.Update(prodRef, "QtdEstoque", novoEstoque);
                transaction.Update(prodRef, "updatedAt", DateTime.UtcNow);
            }

            transaction.Update(solicitacaoRef, "Status", StatusSolicitacao.Aprovada);
            transaction.Update(solicitacaoRef, "updatedAt", DateTime.UtcNow);
        });

        solicitacao.Aprovar();

        return solicitacao;
    }
}
