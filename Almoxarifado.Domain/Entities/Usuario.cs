using System;
using Almoxarifado.Domain.Enums;

namespace Almoxarifado.Domain.Entities;

public sealed class Usuario
{
    public string Id { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Setor { get; private set; } = string.Empty;
    public TipoUsuario Tipo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Usuario() { }

    public Usuario(string id, string nome, string email, string setor, TipoUsuario tipo)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID do usuário é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O Nome do usuário é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O Email do usuário é obrigatório.", nameof(email));

        Id = id;
        Nome = nome.Trim();
        Email = email.Trim().ToLower();

        Setor = string.IsNullOrWhiteSpace(setor)
            ? "Não Informado"
            : setor.Trim();

        Tipo = tipo;
        Ativo = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desativar()
    {
        if (!Ativo)
            return;

        Ativo = false;
        AtualizarData();
    }

    public void Ativar()
    {
        if (Ativo)
            return;

        Ativo = true;
        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}