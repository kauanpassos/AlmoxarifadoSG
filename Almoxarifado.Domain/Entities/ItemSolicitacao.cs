using System;
using Google.Cloud.Firestore;

namespace Almoxarifado.Domain.Entities;

[FirestoreData]
public sealed class ItemSolicitacao
{
    [FirestoreProperty]
    public string Id { get; private set; }

    [FirestoreProperty]
    public string SolicitacaoId { get; private set; }

    [FirestoreProperty]
    public string ProdutoId { get; private set; }

    // --> ADICIONADO: Agora guardamos o nome do produto no Firestore
    [FirestoreProperty]
    public string NomeProduto { get; private set; }

    [FirestoreProperty]
    public int Quantidade { get; private set; }

    // Construtor vazio exigido pelo SDK do Firestore
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
            throw new ArgumentException("A nova quantidade deve ser maior que zero.");

        Quantidade = novaQuantidade;
    }
}