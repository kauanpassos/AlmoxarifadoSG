using Almoxarifado.Domain.Interfaces;

namespace Almoxarifado.Domain;

// Entidade que representa um item no estoque.
// O Domínio é o local onde definimos o que o nosso negócio entende por 'Estoque'.
public class Estoque : IEntity
{
    public int Id { get; set; }

    // Identificação amigável do item.
    public string NomePeca { get; set; } = null!;

    // Detalhes técnicos para especificação rigorosa.
    public string DescricaoTecnica { get; set; } = null!;

    // Saldo atual em unidades.
    public int Quantidade { get; set; }

    // Posição física no armazém (Ex: Corredor A, Prateleira 2).
    public string Localizacao { get; set; } = null!;
}