using MediatR;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Queries;

public record GetSolicitacaoByIdQuery(string Id) : IRequest<SolicitacaoDto?>;
