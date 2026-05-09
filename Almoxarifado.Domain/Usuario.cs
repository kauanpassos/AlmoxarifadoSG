namespace Almoxarifado.Domain;

// Entidade que representa um usuário do sistema.
// Notem que não usamos IEntity aqui porque o ID é gerenciado pelo Auth em alguns cenários.
public class Usuario
{
    public int Id { get; set; }

    // Identificador de login.
    public string? Username { get; set; }

    // Senha (armazenar apenas hashes em produção!).
    public string? Password { get; set; }

    // Nome completo ou exibição.
    public string? Nome { get; set; }
}