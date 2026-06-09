using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Application.DTOs;
using Almoxarifado.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Almoxarifado.App.ViewModels;

public sealed partial class PerfilViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Iniciais))]
    [NotifyPropertyChangedFor(nameof(IsAlmoxarife))]
    [NotifyPropertyChangedFor(nameof(IsColaborador))]
    private UsuarioDto? _usuarioAtual;

    public string Iniciais => ObterIniciais(UsuarioAtual?.Nome);

    public bool IsAlmoxarife => UsuarioAtual?.Tipo is TipoUsuario.Almoxarife;
    public bool IsColaborador => UsuarioAtual?.Tipo is TipoUsuario.Colaborador;

    public PerfilViewModel(IAuthService authService, INavigationService navigationService, IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(authService);
        ArgumentNullException.ThrowIfNull(navigationService);
        ArgumentNullException.ThrowIfNull(dialogService);

        _authService = authService;
        _navigationService = navigationService;
        _dialogService = dialogService;

        UsuarioAtual = UsuarioSessao.UsuarioLogado;
    }

    [RelayCommand]
    private async Task VoltarParaHomeAsync()
    {
        await _navigationService.NavigateToAsync("//HomeColaboradorPage");
    }

    [RelayCommand]
    private async Task SairAsync()
    {
        try
        {
            bool confirmar = await _dialogService.ShowConfirmationAsync(
                "Sair",
                "Deseja realmente encerrar sua sessão?",
                "Sair",
                "Cancelar");

            if (confirmar)
            {
                // CORREÇÃO: Limpar a sessão atual da memória
                UsuarioSessao.UsuarioLogado = null;
                UsuarioAtual = null;

                // Se o seu IAuthService tiver um método para limpar cache/tokens, chame-o aqui. 
                // Exemplo: await _authService.LogoutAsync();

                // Navega para o Login (e como a sessão está nula, ele vai parar lá de verdade)
                await _navigationService.NavigateToLoginAsync();
            }
        }
        catch (Exception)
        {
            await _dialogService.ShowAlertAsync("Erro", "Ocorreu um erro ao tentar sair. Tente novamente.");
        }
    }

    [RelayCommand]
    private async Task VoltarParaFilaAsync()
    {
        await _navigationService.NavigateToAsync("//GestaoFilaPage");
    }

    private string ObterIniciais(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return "US";

        var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length is 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();

        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }
} 