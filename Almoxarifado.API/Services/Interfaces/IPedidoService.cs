using Almoxarifado.Domain;

namespace Almoxarifado.API.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CriarNovoPedido(Pedido pedido);
        Task<IEnumerable<Pedido>> ListarPorUsuario(string usuarioId);
    }
}