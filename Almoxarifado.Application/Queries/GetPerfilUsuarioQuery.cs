using MediatR;

namespace Almoxarifado.Application.Queries;

public class GetPerfilUsuarioQuery : IRequest<PerfilUsuarioDto?>
{
    public string Uid { get; set; }

    public GetPerfilUsuarioQuery(string uid)
    {
        Uid = uid;
    }
}

public class PerfilUsuarioDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Setor { get; set; } = "Não Informado";
    public string Tipo { get; set; } = "Colaborador";
}