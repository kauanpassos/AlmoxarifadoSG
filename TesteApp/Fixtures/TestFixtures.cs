using Almoxarifado.Domain;

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

public static class EstoqueFixture
{
    public static Estoque Criar(int id = 0, string nome = "Correia", string descricao = "Correia de transmissão", int qtd = 50, string local = "A1") => new()
    {
        Id = id,
        NomePeca = nome,
        DescricaoTecnica = descricao,
        Quantidade = qtd,
        Localizacao = local
    };

    public static IReadOnlyList<Estoque> CriarListaPadrao() => new[]
    {
        Criar(1, "Correia", "Correia"),
        Criar(2, "Parafuso", "Parafuso", 200, "A2"),
        Criar(3, "Polia", "Polia", 15, "B1")
    };
}
