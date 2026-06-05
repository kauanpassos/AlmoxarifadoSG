using System.Threading.Tasks;

namespace Almoxarifado.App.Services.Interfaces;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Sim", string cancel = "Não");
}
