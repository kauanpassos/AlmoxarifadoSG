using System;

namespace Almoxarifado.Domain;

public sealed class Estoque
{
    public string ProdutoId { get; }
    public string Localizacao { get; private set; }
    public int QuantidadeAtual { get; private set; }
    public int QuantidadeMinima { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Estoque(string produtoId, string localizacao, int quantidadeAtual, int quantidadeMinima)
    {
        if (string.IsNullOrWhiteSpace(produtoId))
            throw new ArgumentException("O ID do produto é obrigatório.", nameof(produtoId));

        if (quantidadeAtual < 0)
            throw new ArgumentException("A quantidade atual não pode ser negativa.", nameof(quantidadeAtual));

        if (quantidadeMinima < 0)
            throw new ArgumentException("A quantidade mínima não pode ser negativa.", nameof(quantidadeMinima));

        ProdutoId = produtoId;
        Localizacao = string.IsNullOrWhiteSpace(localizacao) ? "Sem Localização" : localizacao;
        QuantidadeAtual = quantidadeAtual;
        QuantidadeMinima = quantidadeMinima;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Adicionar(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade a adicionar deve ser maior que zero.");

        QuantidadeAtual += quantidade;
        AtualizarData();
    }
    public void Retirar(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade a retirar deve ser maior que zero.");

        if (QuantidadeAtual - quantidade < 0)
            throw new InvalidOperationException("Operação negada: Estoque insuficiente para essa retirada.");

        QuantidadeAtual -= quantidade;
        AtualizarData();
    }
    public void MoverPara(string novaLocalizacao)
    {
        if (string.IsNullOrWhiteSpace(novaLocalizacao))
            throw new ArgumentException("A nova localização é obrigatória.");

        Localizacao = novaLocalizacao;
        AtualizarData();
    }
    public bool PrecisaRepor()
    {
        return QuantidadeAtual <= QuantidadeMinima;
    }
    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}