using Almoxarifado.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services.Interfaces;

public interface IFirebaseService
{
    Task<Usuario> GetUsuarioAsync(string uid);

    // Unificado: a listagem visual do ecrã de estoque agora consome diretamente este método
    Task<List<Produto>> GetProdutosAsync();

    Task SalvarProdutoAsync(Produto produto);

    // Quantidade alterada para long para manter o alinhamento com a base de dados
    Task EnviarSolicitacaoAsync(string usuarioId, string sku, long quantidade, string justificativa, string idToken);
}