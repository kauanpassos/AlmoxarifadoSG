using Moq;
using FluentAssertions;
using Almoxarifado.API.Services;
using Almoxarifado.API.Repositories;
using Almoxarifado.Domain;

namespace TesteApp.UnitTests;

[TestClass]
public class PedidoServiceTests
{
    private Mock<IPedidoRepository> _repositoryMock;
    private PedidoService _service;

    // Resetamos o mock e o serviço antes de cada teste para evitar poluição de estado.
    [TestInitialize]
    public void Initialize()
    {
        _repositoryMock = new Mock<IPedidoRepository>();
        _service = new PedidoService(_repositoryMock.Object);
    }

    [TestMethod]
    public async Task CreateAsync_DeveDefinirStatusPendente_AoCriarNovoPedido()
    {
        // Arrange: Criamos um pedido cru e configuramos o mock para retornar o próprio objeto
        var pedido = new Pedido { UsuarioId = 1 };
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Pedido>()))
                       .ReturnsAsync((Pedido p) => p);

        // Act: Disparamos a criação
        var resultado = await _service.CreateAsync(pedido);

        // Assert: Validamos se a regra de negócio de status "Pendente" foi aplicada
        resultado.Status.Should().Be("Pendente");
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_DevePreencherDataAbertura_AoCriarNovoPedido()
    {
        // Arrange: Capturamos o tempo aproximado antes da execução
        var pedido = new Pedido { UsuarioId = 1 };
        var tempoAntes = DateTime.UtcNow;

        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Pedido>()))
                       .ReturnsAsync((Pedido p) => p);

        // Act
        var resultado = await _service.CreateAsync(pedido);

        // Assert: Garantimos que o serviço carimbou a data de abertura corretamente
        resultado.DataAbertura.Should().BeOnOrAfter(tempoAntes);
        resultado.DataAbertura.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [TestMethod]
    public async Task GetByUserIdAsync_DeveRetornarPedidosDoRepositorio()
    {
        // Arrange: Simulamos uma lista de pedidos já existente no banco para um usuário
        var userId = 5;
        var pedidosEsperados = new List<Pedido> 
        { 
            new() { Id = 1, UsuarioId = userId }, 
            new() { Id = 2, UsuarioId = userId } 
        };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                       .ReturnsAsync(pedidosEsperados);

        // Act
        var resultado = await _service.GetByUserIdAsync(userId);

        // Assert: O serviço deve apenas repassar o que o repositório entregou, sem alterar a lista
        resultado.Should().HaveCount(2);
        resultado.Should().BeEquivalentTo(pedidosEsperados);
    }
}
