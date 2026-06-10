using System;
using Google.Cloud.Firestore;

namespace Almoxarifado.Domain.Entities;

[FirestoreData]
public sealed class ItemSolicitacao
{
    [FirestoreProperty("Id")]
    public string Id { get; set; } = null!;

    [FirestoreProperty("SolicitacaoId")]
    public string SolicitacaoId { get; set; } = null!;

    [FirestoreProperty("ProdutoId")]
    public string ProdutoId { get; set; } = null!;

    [FirestoreProperty("NomeProduto")]
    public string NomeProduto { get; set; } = null!;

    [FirestoreProperty("Quantidade")]
    public int Quantidade { get; set; }

    public ItemSolicitacao() { }

    public ItemSolicitacao(string id, string solicitacaoId, string produtoId, string nomeProduto, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID do item é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(solicitacaoId))
            throw new ArgumentException("O ID da solicitação é obrigatório.", nameof(solicitacaoId));

        if (string.IsNullOrWhiteSpace(produtoId))
            throw new ArgumentException("O ID do produto é obrigatório.", nameof(produtoId));

        if (string.IsNullOrWhiteSpace(nomeProduto))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(nomeProduto));

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade solicitada deve ser maior que zero.", nameof(quantidade));

        Id = id;
        SolicitacaoId = solicitacaoId;
        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        Quantidade = quantidade;
    }

    public void AlterarQuantidade(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new ArgumentException("A nova quantidade deve ser maior que zero.", nameof(novaQuantidade));

        Quantidade = novaQuantidade;
    }
}