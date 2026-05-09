using Moq;
using FluentAssertions;
using Almoxarifado.App.ViewModels;
using Almoxarifado.App.Services.Interfaces;
using Almoxarifado.Domain;

namespace TesteApp.UnitTests.Mobile;

[TestClass]
public class LoginViewModelTests
{
    private Mock<IAuthService> _authServiceMock;
    private LoginViewModel _viewModel;

    [TestInitialize]
    public void Initialize()
    {
        _authServiceMock = new Mock<IAuthService>();
        _viewModel = new LoginViewModel(_authServiceMock.Object);
    }

    [TestMethod]
    public void LoginCommand_NaoDeveExecutar_SeEstiverOcupado()
    {
        // Arrange: Simulamos que o app já está processando algo (ex: um clique anterior)
        _viewModel.IsBusy = true;

        // Act: Tentamos disparar o comando de login
        _viewModel.LoginCommand.Execute(null);

        // Assert: O serviço de autenticação NÃO deve ser chamado, para evitar requests duplicados.
        _authServiceMock.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginCommand_DeveDefinirIsBusy_DuranteExecucao()
    {
        // Arrange: Configuramos o mock para validar o estado 'IsBusy' exatamente no meio da execução.
        _authServiceMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(async () => 
            {
                // Verifica se o flag de ocupado subiu enquanto o login "roda".
                _viewModel.IsBusy.Should().BeTrue();
                return await Task.FromResult(new Usuario());
            });

        // Act
        _viewModel.LoginCommand.Execute(null);

        // Assert: Ao final de tudo, o app deve voltar para o estado de "não ocupado".
        _viewModel.IsBusy.Should().BeFalse();
    }
}
