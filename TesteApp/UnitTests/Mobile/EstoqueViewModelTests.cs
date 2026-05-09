using Moq;
using FluentAssertions;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;

namespace TesteApp.UnitTests.Mobile;

[TestClass]
public class EstoqueViewModelTests
{
    private Mock<IEstoqueRepository> _repositoryMock;
    private EstoqueViewModel _viewModel;

    [TestInitialize]
    public void Initialize()
    {
        _repositoryMock = new Mock<IEstoqueRepository>();
        _viewModel = new EstoqueViewModel(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task LoadItemsCommand_DevePopularLista_QuandoSucesso()
    {
        // Arrange: Simulamos que o banco retornou 2 peças de estoque.
        var items = new List<Estoque> 
        { 
            new() { Id = 1, NomePeca = "Peca A" },
            new() { Id = 2, NomePeca = "Peca B" }
        };
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(items);

        // Act: Executamos o comando de carregar a tela.
        _viewModel.LoadItemsCommand.Execute(null);

        // Assert: A lista na ViewModel deve refletir exatamente o que o banco mandou.
        _viewModel.Items.Should().HaveCount(2);
        _viewModel.Items[0].NomePeca.Should().Be("Peca A");
    }

    [TestMethod]
    public async Task LoadItemsCommand_DeveLimparLista_AntesDeCarregarNovosDados()
    {
        // Arrange: Colocamos um item "sujo" na lista para simular um estado anterior.
        _viewModel.Items.Add(new Estoque { Id = 99 });
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Estoque> { new() { Id = 1 } });

        // Act: Ao recarregar...
        _viewModel.LoginCommand.Execute(null);

        // Assert: O item antigo (99) deve sumir, restando apenas o novo (1). 
        // Isso evita duplicidade visual para o usuário.
        _viewModel.Items.Should().HaveCount(1);
        _viewModel.Items[0].Id.Should().Be(1);
    }
}
