using System;

namespace Almoxarifado.Domain;

public sealed class Produto
{
    public string Id { get; }
    public string Nome { get; private set; }
    public string Marca { get; private set; }
    public string Sku { get; private set; }
    public string Unidade { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public Produto(string id, string nome, string marca, string sku, string unidade)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID do produto é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O Nome do produto é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("O SKU do produto é obrigatório.", nameof(sku));

        Id = id;
        Nome = nome.Trim();
        Marca = string.IsNullOrWhiteSpace(marca) ? "Sem Marca" : marca.Trim();
        Sku = sku.Trim();
        Unidade = string.IsNullOrWhiteSpace(unidade) ? "UN" : unidade.Trim();

        Ativo = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Desativar()
    {
        if (!Ativo) return;

        Ativo = false;
        AtualizarData();
    }
    public void Ativar()
    {
        if (Ativo) return;

        Ativo = true;
        AtualizarData();
    }
    public void AtualizarCadastro(string novoNome, string novaMarca, string novoSku)
    {
        if (string.IsNullOrWhiteSpace(novoNome) || string.IsNullOrWhiteSpace(novoSku))
            throw new ArgumentException("Nome e SKU não podem ficar em branco.");

        Nome = novoNome.Trim();
        Marca = string.IsNullOrWhiteSpace(novaMarca) ? "Sem Marca" : novaMarca.Trim();
        Sku = novoSku.Trim();

        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}