namespace Almoxarifado.Domain.Interfaces;
public interface IEngine<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T> where T : class
{
}
