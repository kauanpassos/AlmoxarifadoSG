using Almoxarifado.App.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Services;

/// <summary>
/// Serviço centralizado de navegação que evita conflitos entre Shell, MainPage e histórico.
/// Garante transições seguras entre estados autenticado e não autenticado.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IAuthService _authService;

    public NavigationService(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Navega para a tela de login, limpando completamente o histórico de navegação.
    /// </summary>
    [Obsolete]
    public async Task NavigateToLoginAsync()
    {
        try
        {
            // 1. Faz logout e limpa os dados
            await _authService.LogoutAsync();

            // 2. Reseta a MainPage na Thread Principal
            // Isso limpa todo o histórico e recria o Shell limpo, ativando automaticamente o 
            // redirecionamento padrão do AppShell (que cairá no Login pois não há mais sessão).
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

    /// <summary>
    /// Navega para a área autenticada (home/dashboard genérico).
    /// </summary>
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

    /// <summary>
    /// Navega para uma rota específica.
    /// </summary>
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