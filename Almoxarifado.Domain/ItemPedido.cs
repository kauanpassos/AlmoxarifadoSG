using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Almoxarifado.Domain
{
    [Table("itens")]
    public class ItemPedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        // Eu crio essas duas colunas para servirem de ponte: elas ligam o número do meu Pedido com o número da Peça lá no Estoque.
        [Column("pedido_id")]
        public int PedidoId { get; set; }

        [Column("estoque_id")]
        public int EstoqueId { get; set; }

        [Column("quantidade_solicitada")]
        public int QuantidadeSolicitada { get; set; }
    }
}