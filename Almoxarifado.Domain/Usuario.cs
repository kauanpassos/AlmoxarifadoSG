using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Almoxarifado.Domain.Models // Ajuste se a sua pasta se chamar diferente
{
    [Table("usuarios")] // O nome da sua tabela lá no Supabase
    public class Usuario : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("senha")]
        public string Senha { get; set; }
    }
}