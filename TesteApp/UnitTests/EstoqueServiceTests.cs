using Firebase.Database;
using Almoxarifado.API.Services;
using Almoxarifado.Domain;

namespace TesteApp.UnitTests;

[TestClass]
public class EstoqueServiceTests : IAsyncLifetime
{
    private FirebaseClient _firebaseClient;
    private EstoqueService _service;
    
    // Geramos um namespace aleatório para isolar os dados deste teste dos outros.
    // Assim evitamos que um teste interfira no resultado do outro (Concorrência).
    private readonly string _testNamespace = "test-estoque-" + Guid.NewGuid().ToString("N").Substring(0, 8);

    // Setup inicial: Configura o cliente do Firebase para apontar para o emulador local.
    public async Task InitializeAsync()
    {
        var firebaseUrl = $"http://127.0.0.1:9000?ns={_testNamespace}";
        _firebaseClient = new FirebaseClient(firebaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFunc = async () => "test-secret"
        });
        _service = new EstoqueService(_firebaseClient);
        
        // Populamos o banco com dados fake para os testes terem o que ler.
        await SeedTestData();
    }

    // TearDown: Limpamos os dados criados para manter o ambiente de teste sempre "virgem".
    public async Task DisposeAsync()
    {
        try
        {
            await _firebaseClient.Child("estoque").DeleteAsync();
        }
        catch { }
    }

    // Seed: Dados de exemplo que simulam nosso estoque real.
    private async Task SeedTestData()
    {
        var estoques = new[]
        {
            new { Id = "1", NomePeca = "Correia", DescricaoTecnica = "Correia de transmissão", Quantidade = 50, Localizacao = "A1" },
            new { Id = "2", NomePeca = "Parafuso", DescricaoTecnica = "Parafuso M10", Quantidade = 200, Localizacao = "A2" }
        };

        foreach (var estoque in estoques)
        {
            await _firebaseClient.Child("estoque").Child(estoque.Id).PutAsync(estoque);
        }
    }

    [TestMethod]
    public async Task ObterTodos_DeveFuncionarComFirebase()
    {
        // Verifica se a comunicação básica com o emulador está viva.
        var resultado = await _service.ObterTodos();

        Assert.IsNotNull(resultado);
    }

    [TestMethod]
    public async Task ObterTodos_DeveRetornarEnumeravel()
    {
        // Garante que o retorno é uma lista iterável, como esperado pela UI.
        var resultado = await _service.ObterTodos();

        Assert.IsInstanceOfType(resultado, typeof(IEnumerable<Estoque>));
    }

    [TestMethod]
    public async Task ObterTodos_DeveRetornarEstoquesDoFirebase()
    {
        // Valida se o serviço está realmente lendo os dados que semeamos no Seed.
        var resultado = await _service.ObterTodos();

        Assert.IsTrue(resultado.Count() >= 2);
    }

    [TestMethod]
    public async Task ObterTodos_DeveConterDadosCorretos()
    {
        // Teste de integridade: Checa se os campos (Nome, Quantidade) não estão vindo nulos ou trocados.
        var resultado = await _service.ObterTodos();
        var primeira = resultado.FirstOrDefault();

        Assert.IsNotNull(primeira);
        Assert.AreEqual("Correia", primeira.NomePeca);
        Assert.AreEqual(50, primeira.Quantidade);
    }
}
