using Almoxarifado.API.Repositories;
using Almoxarifado.Domain.Entities; // <-- Agora ele vai usar o Produto daqui
using Almoxarifado.Infrastructure.Repositories;
using Google.Cloud.Firestore;
using System;
using System.IO;

namespace TesteApp.Fixtures;

public sealed partial class DatabaseFixture
{
    private readonly FirestoreDb _firestoreDb;
    private readonly string _testPrefix = "test_" + Guid.NewGuid().ToString("N").Substring(0, 8) + "_";

    // 1. Mudamos de Estoque para Produto
    private readonly FirebaseEngine<Produto> _produtoEngine;
    private readonly FirebaseEngine<Solicitacao> _solicitacaoEngine;

    public DatabaseFixture()
    {
        var credentialsPath = Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json");
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

        var projectId = "almoxarifado-sg";
        _firestoreDb = FirestoreDb.Create(projectId);

        // 2. Instanciamos a engine para o Produto apontando para a coleção "produtos"
        _produtoEngine = new FirebaseEngine<Produto>(_firestoreDb, $"{_testPrefix}produtos");
        _solicitacaoEngine = new FirebaseEngine<Solicitacao>(_firestoreDb, $"{_testPrefix}solicitacoes");
    }
    public FirestoreDb Client => _firestoreDb;
}