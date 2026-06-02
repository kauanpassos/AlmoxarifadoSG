using System.Collections.Generic;
using Almoxarifado.Domain.Entities;
using MediatR;

namespace Almoxarifado.Application.Queries
{
    // Trocamos IEnumerable<Estoque> por IEnumerable<Produto>
    public sealed record GetEstoqueQuery() : IRequest<IEnumerable<Produto>>;
}