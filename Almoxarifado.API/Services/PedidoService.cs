using Almoxarifado.API.Services.Interfaces;
using Almoxarifado.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.API.Services
{
    public class PedidoService : IPedidoService
    {
        public async Task<Solicitacao> CriarNovoPedido(Solicitacao pedido)
        {
            return await Task.FromResult(pedido);
        }

        public async Task<IEnumerable<Solicitacao>> ListarPorUsuario(string usuarioId)
        {
            return await Task.FromResult(new List<Solicitacao>());
        }
    }
}