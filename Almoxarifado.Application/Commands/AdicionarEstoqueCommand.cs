using MediatR;
using Almoxarifado.Application.DTOs;

namespace Almoxarifado.Application.Commands;

public record AdicionarEstoqueCommand(string ProdutoId, long Quantidade) : IRequest<ProdutoDto>;
