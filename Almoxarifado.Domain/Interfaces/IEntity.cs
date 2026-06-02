namespace Almoxarifado.Domain.Interfaces;

public interface IEntity
{
    // Alterado de int para string para suportar os IDs do Firestore
    string Id { get; }
}