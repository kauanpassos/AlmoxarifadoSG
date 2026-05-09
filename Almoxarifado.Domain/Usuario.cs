using System;

namespace Almoxarifado.Domain;

public class Usuario
{
    public string Id { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Setor { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = string.Empty; // Ex: "Admin", "Operador"
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Construtor completo para criação/restauração
    public Usuario(string id, string nome, string email, string setor, string tipo)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Setor = setor;
        Tipo = tipo;
        Ativo = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Construtor vazio para serialização se necessário
    public Usuario() { }

    public static void ValidarRegrasDeSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 6)
            throw new ArgumentException("A senha deve ter pelo menos 6 caracteres.");
    }

    public void Desativar()
    {
        Ativo = false;
        UpdatedAt = DateTime.UtcNow;
    }
}