namespace Almoxarifado.App.Services.Interfaces;

public interface INavigationService
{
    Task NavigateToLoginAsync();
    Task NavigateToHomeAsync();
    Task NavigateToAsync(string route);
}