using MediatR;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Application.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Almoxarifado.Application.Handlers;

public sealed class CreateSolicitacaoHandler(IWriteOnlyRepository<Solicitacao> repository)
    : IRequestHandler<CreateSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> Handle(CreateSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        var solicitacao = new Solicitacao(Guid.NewGuid().ToString(), request.UsuarioId, request.Observacao);

        foreach (var item in request.Itens ?? [])
        {
            // CORREÇÃO AQUI: Passando o item.NomeProduto como quarto parâmetro
            solicitacao.AdicionarItem(new ItemSolicitacao(
                Guid.NewGuid().ToString(),
                solicitacao.Id,
                item.Sku,
                item.NomeProduto, // <-- O nome que o construtor estava exigindo!
                item.Quantidade
            ));
        }

        await repository.AddAsync(solicitacao);

        return solicitacao.ToDto();
    }
}