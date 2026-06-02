using Almoxarifado.Domain.Entities;
using System;
using System.Collections.Generic;

namespace TesteApp.Fixtures;

public static class SolicitacaoFixture
{
    private const string StatusPendente = "Pendente";

    public static Solicitacao Criar(string id = "test-id", string usuarioId = "user-1", string observacao = "Test")
    {
        return new Solicitacao(id, usuarioId, observacao);
    }

    public static IReadOnlyList<Solicitacao> CriarLista(int quantidade = 3, string usuarioId = "user-1")
    {
        var lista = new List<Solicitacao>();
        for (int i = 1; i <= quantidade; i++)
        {
            lista.Add(Criar($"id-{i}", usuarioId, $"Solicitação {i}"));
        }
        return lista.AsReadOnly();
    }
}

public static class ProdutoFixture
{
    // Ajustado os parâmetros padrão para bater com a ordem e tipos corretos do construtor
    public static Produto Criar(
        string id = "prod-test-id",
        long numCode = 1001,
        string nome = "Correia",
        string descricao = "Correia de transmissão",
        string localizacao = "A1",
        long qtd = 50)
    {
        // Enviando os argumentos na ordem exata identificada pelo compilador
        return new Produto(id, numCode, nome, descricao, localizacao, qtd);
    }

    public static IReadOnlyList<Produto> CriarListaPadrao() => new[]
    {
        Criar("id-1", 1001, "Correia", "Correia de transmissão", "A1", 50),
        Criar("id-2", 1002, "Parafuso", "Parafuso M10", "A2", 200),
        Criar("id-3", 1003, "Polia", "Polia 30mm", "B1", 15)
    };
}