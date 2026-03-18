using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Almoxarifado.Domain;

[Table("usuarios")]
public class Usuario : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [Column("password")]
    public string? Password { get; set; }
}