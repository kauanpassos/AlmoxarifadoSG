using Firebase.Database;
using Almoxarifado.API.Repositories;
using Almoxarifado.Domain;

namespace TesteApp.Fixtures;

public sealed partial class DatabaseFixture
{
    private readonly FirebaseClient _firebase;
    private readonly string _testNamespace = "almoxarifado-test-" + Guid.NewGuid().ToString("N").Substring(0, 8);
    private readonly FirebaseEngine<Estoque> _estoqueEngine;
    private readonly FirebaseEngine<Pedido> _pedidoEngine;

    public DatabaseFixture()
    {
        var firebaseUrl = $"http://127.0.0.1:9000?ns={_testNamespace}";
        _firebase = new FirebaseClient(firebaseUrl, new FirebaseOptions
        {
            AuthTokenAsyncFunc = async () => "test-secret-key"
        });
        
        _estoqueEngine = new FirebaseEngine<Estoque>(_firebase, "estoque");
        _pedidoEngine = new FirebaseEngine<Pedido>(_firebase, "pedidos");
    }

    public FirebaseClient Client => _firebase;
}
