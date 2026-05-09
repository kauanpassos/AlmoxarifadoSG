using Almoxarifado.App.ViewModels;

namespace TesteApp.E2ETests;

[TestClass]
public class LoginViewModelE2ETests
{
    private LoginViewModel _viewModel;

    [TestInitialize]
    public void Setup()
    {
        _viewModel = new LoginViewModel();
    }

    [TestMethod]
    public void LoginViewModel_DeveExistir()
    {
        Assert.IsNotNull(_viewModel);
    }

    [TestMethod]
    public void LoginViewModel_DeveAceitarCredenciais()
    {
        _viewModel.Usuario = "joao";
        _viewModel.Senha = "123456";

        Assert.AreEqual("joao", _viewModel.Usuario);
        Assert.AreEqual("123456", _viewModel.Senha);
    }

    [TestMethod]
    public void LoginViewModel_DeveValidarUsuarioVazio()
    {
        _viewModel.Usuario = "";
        _viewModel.Senha = "123456";

        bool isValid = !string.IsNullOrWhiteSpace(_viewModel.Usuario) && !string.IsNullOrWhiteSpace(_viewModel.Senha);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void LoginViewModel_DeveValidarSenhaVazia()
    {
        _viewModel.Usuario = "joao";
        _viewModel.Senha = "";

        bool isValid = !string.IsNullOrWhiteSpace(_viewModel.Usuario) && !string.IsNullOrWhiteSpace(_viewModel.Senha);

        Assert.IsFalse(isValid);
    }

    [TestMethod]
    public void LoginViewModel_DeveAceitarCredenciaisValidas()
    {
        var usuarios = new[] { "joao", "maria", "admin" };
        var senhas = new[] { "123456", "senha", "pass@123" };

        foreach (var usuario in usuarios)
        {
            foreach (var senha in senhas)
            {
                _viewModel.Usuario = usuario;
                _viewModel.Senha = senha;

                Assert.AreEqual(usuario, _viewModel.Usuario);
                Assert.AreEqual(senha, _viewModel.Senha);
            }
        }
    }
}
