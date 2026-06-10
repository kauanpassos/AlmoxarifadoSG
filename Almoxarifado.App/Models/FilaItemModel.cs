using Almoxarifado.Application.DTOs;

namespace Almoxarifado.App.Models;

public class FilaItemModel
{
    public string SolicitacaoId { get; set; } = string.Empty;
    public string UsuarioSolicitante { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public long Quantidade { get; set; }
    public string Status { get; set; } = string.Empty;
    public SolicitacaoDto SolicitacaoOriginal { get; set; } = null!;
}