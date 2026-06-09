using Almoxarifado.App.Services.Interfaces;
using Microsoft.Maui.Controls;

namespace Almoxarifado.App.Services;

public sealed class NavigationService(IAuthService authService, IServiceProvider serviceProvider) : INavigationService
{
    public async Task NavigateToLoginAsync()
    {
        try
        {
            await authService.LogoutAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Microsoft.Maui.Controls.Application.Current is not null)
                {
                    var window = Microsoft.Maui.Controls.Application.Current.Windows.FirstOrDefault();
                    if (window is not null)
                    {
                        window.Page = serviceProvider.GetRequiredService<AppShell>();
                    }
                }
            });
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Falha crítica ao redirecionar para a tela de login. Tente reiniciar o aplicativo.");
        }
    }

    public Task NavigateToHomeAsync() => NavigateToAsync("//HomeColaboradorPage");

    public async Task NavigateToAsync(string route)
    {
        try
        {
            if (Shell.Current is not null)
            {
                await Shell.Current.GoToAsync(route, animate: true);
            }
            else
            {
                throw new InvalidOperationException("A infraestrutura de navegação principal não está pronta.");
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException($"Não foi possível navegar para o destino solicitado. Tente novamente.");
        }
    }
}