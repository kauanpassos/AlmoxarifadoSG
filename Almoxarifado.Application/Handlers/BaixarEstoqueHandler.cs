using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Almoxarifado.Application.Handlers;

public sealed class BaixarEstoqueHandler(
    IReadOnlyRepository<Produto> readRepository,
    IWriteOnlyRepository<Produto> writeRepository)
    : IRequestHandler<BaixarEstoqueCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(BaixarEstoqueCommand request, CancellationToken cancellationToken)
    {
        var produto = await readRepository.GetByIdAsync(request.ProdutoId)
            ?? throw new KeyNotFoundException($"Produto '{request.ProdutoId}' não encontrado.");

        produto.BaixarEstoque(request.Quantidade);

        await writeRepository.UpdateAsync(produto);

        return produto.ToDto();
    }
}
