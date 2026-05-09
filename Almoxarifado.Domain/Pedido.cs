using Almoxarifado.Domain.Interfaces;

namespace Almoxarifado.Domain;

// Entidade central de Pedido.
// NOTA PARA O TIME: Mantemos esta classe como um POCO (Plain Old CLR Object), 
// livre de atributos de infraestrutura (como Supabase ou Entity Framework). 
// Isso garante que o nosso Domínio não mude se trocarmos o banco de dados.
public class Pedido : IEntity
{
    // Identificador único.
    public int Id { get; set; }

    // Vinculação com o usuário solicitante.
    public int UsuarioId { get; set; }

    // Controle de fluxo do pedido.
    public string Status { get; set; } = null!;

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    // Registro cronológico da criação.
    public DateTime DataAbertura { get; set; }
}