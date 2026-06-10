using MediatR;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Commands;

public record BaixarEstoqueCommand(string ProdutoId, long Quantidade) : IRequest<ProdutoDto>;
