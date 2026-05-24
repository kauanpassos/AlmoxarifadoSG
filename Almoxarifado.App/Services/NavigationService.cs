using Almoxarifado.App.Services.Interfaces;

namespace Almoxarifado.App.Services;

public class NavigationService : INavigationService
{
    private readonly IAuthService _authService;

    public NavigationService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task NavigateToLoginAsync()
    {
        try
        {
            await _authService.LogoutAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Microsoft.Maui.Controls.Application.Current != null)
                {
                    Microsoft.Maui.Controls.Application.Current.MainPage = new AppShell();
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro na navegação para login: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public async Task NavigateToHomeAsync()
    {
        try
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("//HomeColaboradorPage", animate: true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao navegar para home: {ex.Message}");
        }
    }

    public async Task NavigateToAsync(string route)
    {
        try
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync(route);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao navegar para {route}: {ex.Message}");
        }
    }
}