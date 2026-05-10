using System.Collections.Generic;
using MediatR;
using Almoxarifado.Domain;

namespace Almoxarifado.Application.Queries;

// Consulta para obter todas as solicitações vinculadas a um usuário.
public record GetSolicitacoesByUserQuery(string UsuarioId) : IRequest<IEnumerable<Solicitacao>>;