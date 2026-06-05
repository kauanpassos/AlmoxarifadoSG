using Almoxarifado.App.Services;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.App.Views;
using Almoxarifado.Domain.Entities;
using Almoxarifado.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace Almoxarifado.App.ViewModels;

public partial class GestaoFilaViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private readonly List<Solicitacao> _filaCompleta = new();

    public ObservableCollection<Solicitacao> Solicitacoes { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool isBusy;

    [ObservableProperty]
    private string textoPesquisa = string.Empty;

    [ObservableProperty]
    private string filtroStatus = "Todos";

    public bool IsEmpty => Solicitacoes.Count == 0 && !IsBusy;

    public string IniciaisUsuario
    {
        get
        {
            var nome = UsuarioSessao.UsuarioLogado?.Nome;
            if (string.IsNullOrWhiteSpace(nome)) return "US";
            var partes = nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
            return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
        }
    }

    public GestaoFilaViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [RelayCommand]
    private async Task IrParaPerfilAsync()
    {
        await Shell.Current.GoToAsync(nameof(PerfilPage));
    }

    [RelayCommand]
    private async Task IrParaEstoqueAsync()
    {
        await Shell.Current.GoToAsync(nameof(EstoquePage));
    }

    partial void OnTextoPesquisaChanged(string value) => AplicarFiltros();

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Solicitacoes.Clear();

            var usuario = UsuarioSessao.UsuarioLogado;
            if (usuario == null) return;

            var idToken = await SecureStorage.Default.GetAsync("auth_token") ?? string.Empty;
            var url = "api/Solicitacao";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar as solicitações: {err}", "OK");
                return;
            }

            var content = await resp.Content.ReadAsStringAsync();
            var dados = JsonSerializer.Deserialize<List<Solicitacao>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Solicitacao>();

            _filaCompleta.Clear();
            _filaCompleta.AddRange(dados);
            AplicarFiltros();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro ao carregar", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void MudarFiltro(string status)
    {
        FiltroStatus = status;
        AplicarFiltros();
    }

    [RelayCommand]
    private async Task AprovarAsync(Solicitacao solicitacao)
    {
        await AlterarStatusAsync(solicitacao, "Aprovada");
    }

    [RelayCommand]
    private async Task RecusarAsync(Solicitacao solicitacao)
    {
        await AlterarStatusAsync(solicitacao, "Recusada");
    }

    private async Task AlterarStatusAsync(Solicitacao solicitacao, string novoStatus)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var idToken = await SecureStorage.Default.GetAsync("auth_token") ?? string.Empty;
            var url = $"api/Solicitacao/{solicitacao.Id}/status";
            var body = JsonSerializer.Serialize(new { NovoStatus = novoStatus });

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var resp = await _httpClient.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível atualizar a solicitação: {err}", "OK");
                return;
            }

            await LoadAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro inesperado", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltros()
    {
        var temp = _filaCompleta.AsEnumerable();

        if (FiltroStatus != "Todos")
            temp = temp.Where(s => s.Status.Equals(FiltroStatus, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(TextoPesquisa))
            temp = temp.Where(s =>
                s.Id.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                (s.Observacao != null && s.Observacao.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                (s.Sku != null && s.Sku.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase)));

        Solicitacoes.Clear();
        foreach (var item in temp)
            Solicitacoes.Add(item);

        OnPropertyChanged(nameof(IsEmpty));
    }
}