using System.Collections.Generic;
using MediatR;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Queries;
public record GetSolicitacoesByUserQuery(string UsuarioId) : IRequest<IEnumerable<SolicitacaoDto>>;