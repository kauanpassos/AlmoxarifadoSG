using Almoxarifado.App.Services.Interfaces;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services;

public sealed class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        if (Shell.Current is null) return;

        await Shell.Current.DisplayAlert(title, message, cancel);
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Sim", string cancel = "Não")
    {
        if (Shell.Current is null) return false;

        return await Shell.Current.DisplayAlert(title, message, accept, cancel);
    }
}