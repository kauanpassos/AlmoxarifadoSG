using Almoxarifado.Domain;
using Supabase;

namespace Almoxarifado.API.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly Client _supabase;

        // Eu recebo o cliente do Supabase aqui pela Injeção de Dependência
        public PedidoService(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Pedido> CriarNovoPedido(Pedido pedido)
        {
            // Eu garanto que a data e o status fiquem preenchidos automaticamente
            pedido.DataAbertura = DateTime.UtcNow;
            pedido.Status = "Pendente";

            var response = await _supabase.From<Pedido>().Insert(pedido);

            // Eu retorno o pedido que foi criado lá no banco
            return response.Models.FirstOrDefault() ?? pedido;
        }

        public async Task<IEnumerable<Pedido>> ListarPorUsuario(string usuarioId)
        {
            // COMO EU RESOLVO O ERRO: Transformo o texto que chegou em um número de verdade!
            int idNumerico = int.Parse(usuarioId);

            var response = await _supabase.From<Pedido>()
                // Agora sim! Eu comparo o número do banco (x.UsuarioId) com o número que eu acabei de converter
                .Where(x => x.UsuarioId == idNumerico)
                .Get();

            return response.Models;
        }
    }
}