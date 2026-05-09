using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Almoxarifado.App.ViewModels;

// ViewModel responsável pela tela de listagem de estoque.
// Utilizamos o padrão MVVM para separar a lógica de apresentação da UI (XAML).
public sealed class EstoqueViewModel : BaseViewModel
{
    private readonly IReadOnlyRepository<Estoque> _repository;
    
    // Coleção observável que notifica a tela automaticamente quando itens são adicionados ou removidos.
    public ObservableCollection<Estoque> Items { get; } = new();
    
    // Comando para disparar a carga de dados (bindado ao RefreshView ou Botão na tela).
    public ICommand LoadItemsCommand { get; }

    public EstoqueViewModel(IReadOnlyRepository<Estoque> repository)
    {
        _repository = repository;
        Title = "Estoque de Peças";
        LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
    }

    private async Task ExecuteLoadItemsCommand()
    {
        // Evita chamadas duplicadas enquanto uma já está em andamento.
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            
            // Busca as peças diretamente do motor Firebase unificado.
            var items = await _repository.GetAllAsync();

            // DICA PARA OS JUNIORES: Sempre limpem a coleção antes de recarregar para evitar duplicatas na tela.
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            // Tratamento de erro amigável para o usuário final.
            await Application.Current!.MainPage!.DisplayAlert("Erro", $"Não foi possível carregar as peças: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
