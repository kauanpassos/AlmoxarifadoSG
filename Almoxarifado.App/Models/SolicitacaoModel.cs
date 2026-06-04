namespace Almoxarifado.App.Models;

public sealed class SolicitacaoModel
{
    public string NomePeca { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public string NomeStatus { get; set; } = string.Empty;
}
