using System;
using System.Text.RegularExpressions;

namespace Almoxarifado.Domain;

public sealed class Usuario
{
    public string Id { get; }
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string Setor { get; private set; }
    public string Tipo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public Usuario(string id, string nome, string email, string setor, string tipo)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID do usuário é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O Nome é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O Email é obrigatório.", nameof(email));

        Id = id;
        Nome = nome.Trim();
        Email = email.Trim();
        Setor = string.IsNullOrWhiteSpace(setor) ? "Não Informado" : setor.Trim();
        Tipo = string.IsNullOrWhiteSpace(tipo) ? "Funcionario" : tipo.Trim();

        Ativo = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public static void ValidarRegrasDeSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("A senha é obrigatória.");

        if (senha.Length < 6)
            throw new ArgumentException("A senha deve ter no mínimo 6 caracteres.");
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

    public void MudarSetor(string novoSetor)
    {
        if (string.IsNullOrWhiteSpace(novoSetor))
            throw new ArgumentException("O novo setor não pode ser vazio.");

        Setor = novoSetor.Trim();
        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}