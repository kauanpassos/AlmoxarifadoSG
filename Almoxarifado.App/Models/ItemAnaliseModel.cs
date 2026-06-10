using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Almoxarifado.App.Models;

public partial class ItemAnaliseModel : ObservableObject
{
    public string Sku { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public long Quantidade { get; set; }

    private string _estado = "Aguardando";
    public string Estado
    {
        get => _estado;
        set => SetProperty(ref _estado, value);
    }

    public Action? AoAlterarEstado { get; set; }

    [RelayCommand]
    private void MarcarValidado()
    {
        Estado = "Validado";
        AoAlterarEstado?.Invoke();
    }

    [RelayCommand]
    private void MarcarEmFalta()
    {
        Estado = "EmFalta";
        AoAlterarEstado?.Invoke();
    }
}