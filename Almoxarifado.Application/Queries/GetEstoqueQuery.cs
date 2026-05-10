using System.Collections.Generic;
using Almoxarifado.Domain;
using MediatR;

namespace Almoxarifado.Application.Queries
{
    // Query simples para solicitar a lista de Estoque via MediatR
    public sealed record GetEstoqueQuery() : IRequest<IEnumerable<Estoque>>;
}