using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;

namespace Almoxarifado.Application.Handlers;

// Handler responsável pela criação de pedidos via MediatR.
// Utilizamos Primary Constructors (C# 12) para simplificar a injeção de dependência.
public sealed class CreatePedidoHandler(IWriteOnlyRepository<Pedido> repository) : IRequestHandler<CreatePedidoCommand, Pedido>
{
    public async Task<Pedido> Handle(CreatePedidoCommand request, CancellationToken cancellationToken)
    {
        // Delegamos a persistência para o repositório genérico.
        // O uso do IWriteOnlyRepository (ISP) garante que este Handler não tenha acesso a métodos de leitura desnecessários.
        await repository.AddAsync(request.Pedido);
        return request.Pedido;
    }
}
