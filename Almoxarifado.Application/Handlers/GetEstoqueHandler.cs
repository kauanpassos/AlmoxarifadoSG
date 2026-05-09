using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Queries;

namespace Almoxarifado.Application.Handlers;

// Handler responsável por recuperar a listagem completa de itens do almoxarifado.
// Notem que utilizamos IReadOnlyRepository (ISP) porque esta é uma operação de consulta (Query).
public sealed class GetEstoqueHandler(IReadOnlyRepository<Estoque> repository) : IRequestHandler<GetEstoqueQuery, IEnumerable<Estoque>>
{
    public async Task<IEnumerable<Estoque>> Handle(GetEstoqueQuery request, CancellationToken cancellationToken)
    {
        // Delegamos a busca para o motor genérico de leitura.
        // Se precisarmos de filtros ou ordenação no futuro, este é o lugar para implementar.
        return await repository.GetAllAsync();
    }
}
