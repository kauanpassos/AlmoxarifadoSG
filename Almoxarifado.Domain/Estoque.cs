using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Almoxarifado.Domain
{
    [Table("estoque")]
    public class Estoque : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nome_peca")]
        public string NomePeca { get; set; }

        // Eu deixo a descrição como string para armazenar os detalhes técnicos que eu cadastrei na nuvem.
        [Column("descricao_tecnica")]
        public string DescricaoTecnica { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("localizacao")]
        public string Localizacao { get; set; }
    }
}