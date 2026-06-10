using MediatR;
using System.Collections.Generic;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Queries;

public record GetTodasSolicitacoesQuery : IRequest<IEnumerable<SolicitacaoDto>>;