using Almoxarifado.Domain.Interfaces;
using Google.Cloud.Firestore;
using System;

namespace Almoxarifado.Domain.Entities;

[FirestoreData]
public sealed class Produto : IEntity
{
    public Produto() { }

    [FirestoreDocumentId]
    public string Id { get; private set; } = string.Empty;

    [FirestoreProperty("numCode")]
    public long NumCode { get; private set; }

    [FirestoreProperty("nome")]
    public string Nome { get; private set; } = string.Empty;

    [FirestoreProperty("nomeLower")]
    public string NomeLower { get; private set; } = string.Empty;

    [FirestoreProperty("categoria")]
    public string Categoria { get; private set; } = string.Empty;

    [FirestoreProperty("unidadeMedida")]
    public string UnidadeMedida { get; private set; } = string.Empty;

    [FirestoreProperty("qtdEstoque")]
    public long QtdEstoque { get; private set; }

    [FirestoreProperty("estoqueMinimo")]
    public long EstoqueMinimo { get; private set; }

    [FirestoreProperty("ativo")]
    public bool Ativo { get; private set; }

    [FirestoreProperty("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [FirestoreProperty("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    public Produto(string id, long numCode, string nome, string categoria, string unidadeMedida, long estoqueMinimo)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID do produto é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O Nome do produto é obrigatório.", nameof(nome));

        if (numCode <= 0)
            throw new ArgumentException("O NumCode deve ser maior que zero.", nameof(numCode));

        Id = id;
        NumCode = numCode;
        Nome = nome.Trim();
        NomeLower = Nome.ToLowerInvariant();
        Categoria = string.IsNullOrWhiteSpace(categoria) ? "Geral" : categoria.Trim();
        UnidadeMedida = string.IsNullOrWhiteSpace(unidadeMedida) ? "UNI" : unidadeMedida.Trim();

        QtdEstoque = 0;
        EstoqueMinimo = estoqueMinimo < 0 ? 0 : estoqueMinimo;

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

    public void AdicionarEstoque(long quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade a adicionar deve ser maior que zero.");

        QtdEstoque += quantidade;
        AtualizarData();
    }

    public void BaixarEstoque(long quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade a baixar deve ser maior que zero.");

        if (QtdEstoque - quantidade < 0)
            throw new InvalidOperationException("Estoque insuficiente de material.");

        QtdEstoque -= quantidade;
        AtualizarData();
    }

    public void AtualizarCadastro(string novoNome, string novaCategoria, long novoEstoqueMinimo)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("O Nome não pode ficar em branco.");

        Nome = novoNome.Trim();
        NomeLower = Nome.ToLowerInvariant();
        Categoria = string.IsNullOrWhiteSpace(novaCategoria) ? "Geral" : novaCategoria.Trim();
        EstoqueMinimo = novoEstoqueMinimo < 0 ? 0 : novoEstoqueMinimo;

        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}