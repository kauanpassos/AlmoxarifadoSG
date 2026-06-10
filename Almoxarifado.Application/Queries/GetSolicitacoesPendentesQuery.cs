using MediatR;
using System.Collections.Generic;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Queries;

public record GetSolicitacoesPendentesQuery : IRequest<IEnumerable<SolicitacaoDto>>;