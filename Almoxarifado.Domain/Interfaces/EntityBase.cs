namespace Almoxarifado.Domain.Interfaces;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// ISOLAMENTO: Usamos uma classe abstrata em vez de interface para reduzir o acoplamento 
/// de contratos em restrições genéricas de infraestrutura.
/// </summary>
public abstract class EntityBase
{
    public abstract int Id { get; set; }
}
