using System;

namespace Almoxarifado.Domain;

public sealed class ItemSolicitacao
{
    public string Id { get; }
    public string SolicitacaoId { get; }
    public string ProdutoId { get; }
    public int Quantidade { get; private set; }
    public ItemSolicitacao(string id, string solicitacaoId, string produtoId, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID do item é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(solicitacaoId))
            throw new ArgumentException("O ID da solicitação é obrigatório.", nameof(solicitacaoId));

        if (string.IsNullOrWhiteSpace(produtoId))
            throw new ArgumentException("O ID do produto é obrigatório.", nameof(produtoId));

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade solicitada deve ser maior que zero.", nameof(quantidade));

        Id = id;
        SolicitacaoId = solicitacaoId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
    }
    public void AlterarQuantidade(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new ArgumentException("A nova quantidade deve ser maior que zero.");

        Quantidade = novaQuantidade;
    }
}