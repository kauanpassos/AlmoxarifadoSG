using Almoxarifado.Domain.Interfaces;

namespace Almoxarifado.Domain.Entities;
public class Estoque : IEntity
{
    public int Id { get; set; }
    public string NomePeca { get; set; } = null!;
    public string DescricaoTecnica { get; set; } = null!;
    public int Quantidade { get; set; }
    public string Localizacao { get; set; } = null!;
}