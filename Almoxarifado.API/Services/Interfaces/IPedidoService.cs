using Almoxarifado.Domain;

namespace Almoxarifado.API.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<Solicitacao> CriarNovoPedido(Solicitacao pedido);
        Task<IEnumerable<Solicitacao>> ListarPorUsuario(string usuarioId);
    }
}