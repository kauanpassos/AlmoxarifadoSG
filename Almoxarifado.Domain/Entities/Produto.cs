using Almoxarifado.Domain.Interfaces;
using System;

namespace Almoxarifado.Domain.Entities;

public sealed class Produto : IEntity
{
    public Produto() { }
    public string Id { get; set; } = string.Empty;
    public long NumCode { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeLower { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public long QtdEstoque { get; set; }
    public long EstoqueMinimo { get; set; }
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Produto(string id, long numCode, string nome, string categoria, string unidadeMedida, long estoqueMinimo)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("O ID do produto é obrigatório.", nameof(id));
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("O Nome do produto é obrigatório.", nameof(nome));
        if (numCode <= 0) throw new ArgumentException("O NumCode deve ser maior que zero.", nameof(numCode));

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
        if (quantidade <= 0) throw new ArgumentException("A quantidade a adicionar deve ser maior que zero.");
        QtdEstoque += quantidade;
        AtualizarData();
    }

    public void BaixarEstoque(long quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("A quantidade a baixar deve ser maior que zero.");
        if (QtdEstoque - quantidade < 0) throw new InvalidOperationException("Estoque insuficiente.");

        QtdEstoque -= quantidade;
        AtualizarData();
    }

    public void AtualizarCadastro(string novoNome, string novaCategoria, long novoEstoqueMinimo)
    {
        if (string.IsNullOrWhiteSpace(novoNome)) throw new ArgumentException("O Nome não pode ficar em branco.");

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