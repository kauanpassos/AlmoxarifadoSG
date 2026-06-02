using Almoxarifado.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services.Interfaces;

public interface IFirebaseService
{
    Task<Usuario> GetUsuarioAsync(string uid);
    Task<List<Produto>> GetProdutosAsync();
    Task SalvarProdutoAsync(Produto produto);
    Task EnviarSolicitacaoAsync(string usuarioId, string sku, long quantidade, string justificativa, string idToken);
}