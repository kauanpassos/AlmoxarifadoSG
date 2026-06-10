using Almoxarifado.App.Services.Interfaces;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

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
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Falha crítica ao redirecionar para a tela de login. Detalhe: {ex.Message}");
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
                throw new InvalidOperationException("A infraestrutura de navegação principal (Shell) não está pronta.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Não foi possível navegar para a rota '{route}'. Detalhes do MAUI: {ex.Message}");
        }
    }
}