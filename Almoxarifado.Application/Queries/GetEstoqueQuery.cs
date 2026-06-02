using System.Collections.Generic;
using Almoxarifado.Domain.Entities;
using MediatR;

namespace Almoxarifado.Application.Queries
{
    public sealed record GetEstoqueQuery() : IRequest<IEnumerable<Produto>>;
}