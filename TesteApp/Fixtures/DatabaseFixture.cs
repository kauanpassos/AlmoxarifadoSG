using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Entities;
namespace TesteApp.Fixtures;

public sealed partial class DatabaseFixture
{
    private readonly FirebaseClient _firebase;
    private readonly string _testNamespace = "almoxarifado-test-" + Guid.NewGuid().ToString("N").Substring(0, 8);
    private readonly FirebaseEngine<Estoque> _estoqueEngine;
    private readonly FirebaseEngine<Solicitacao> _solicitacaoEngine;

    public DatabaseFixture()
    {
        var firebaseUrl = $"http://127.0.0.1:9000?ns={_testNamespace}";
        _firebase = new FirebaseClient(firebaseUrl);

        _estoqueEngine = new FirebaseEngine<Estoque>(_firebase, "estoque");
        _solicitacaoEngine = new FirebaseEngine<Solicitacao>(_firebase, "solicitacoes");
    }

    public FirebaseClient Client => _firebase;
}
