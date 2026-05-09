using Almoxarifado.Domain;

namespace TesteApp.Fixtures;

public static class PedidoFixture
{
    private const string StatusPendente = "Pendente";

    public static Pedido Criar(int id = 0, int usuarioId = 1, string status = StatusPendente) => new()
    {
        Id = id,
        UsuarioId = usuarioId,
        Status = status,
        DataAbertura = DateTime.UtcNow
    };

    public static IReadOnlyList<Pedido> CriarLista(int quantidade = 3, int usuarioId = 1) =>
        Enumerable.Range(1, quantidade)
                  .Select(i => Criar(i, usuarioId))
                  .ToArray();
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
