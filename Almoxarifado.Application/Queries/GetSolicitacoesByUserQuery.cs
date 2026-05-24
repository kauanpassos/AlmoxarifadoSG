using System.Collections.Generic;
using MediatR;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.Application.Queries;
public record GetSolicitacoesByUserQuery(string UsuarioId) : IRequest<IEnumerable<Solicitacao>>;