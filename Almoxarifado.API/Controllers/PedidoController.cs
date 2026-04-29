using Microsoft.AspNetCore.Mvc;
using Almoxarifado.Domain;
using Almoxarifado.API.Services.Interfaces;

namespace Almoxarifado.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost]
        public async Task<ActionResult<Solicitacao>> CriarPedido([FromBody] Solicitacao novoPedido)
        {
            try
            {
                var pedidoCriado = await _pedidoService.CriarNovoPedido(novoPedido);
                return Ok(pedidoCriado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao registrar pedido: {ex.Message}");
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetPedidosPorUsuario(string usuarioId)
        {
            var pedidos = await _pedidoService.ListarPorUsuario(usuarioId);
            return Ok(pedidos);
        }
    }
}