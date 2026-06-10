using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Almoxarifado.Application.Handlers;

public sealed class AdicionarEstoqueHandler(
    IReadOnlyRepository<Produto> readRepository,
    IWriteOnlyRepository<Produto> writeRepository)
    : IRequestHandler<AdicionarEstoqueCommand, ProdutoDto>
{
    public async Task<ProdutoDto> Handle(AdicionarEstoqueCommand request, CancellationToken cancellationToken)
    {
        var produto = await readRepository.GetByIdAsync(request.ProdutoId)
            ?? throw new KeyNotFoundException($"Produto '{request.ProdutoId}' não encontrado.");

        produto.AdicionarEstoque(request.Quantidade);

        await writeRepository.UpdateAsync(produto);

        return produto.ToDto();
    }
}
