namespace Almoxarifado.Domain.Interfaces;

// Interface de unificação para motores de infraestrutura.
// Herda de IReadOnlyRepository e IWriteOnlyRepository para suportar segregação de interface (ISP).
// As implementações técnicas (Firebase, SQL, EF) devem satisfazer este contrato.
public interface IEngine<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T> where T : class
{
}
