using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Almoxarifado.Domain
{
    [Table("pedidos")]
    public class Pedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        // Aqui eu guardo o ID do usuário que fez o pedido, criando a relação entre quem pediu e o protocolo.
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("data_inicio")]
        public DateTime? DataInicio { get; set; }

        [Column("data_fim")]
        public DateTime? DataFim { get; set; }

        // Adicionado o mapeamento da coluna para o Supabase reconhecer!
        [Column("data_abertura")]
        public DateTime DataAbertura { get; set; }
    }
}