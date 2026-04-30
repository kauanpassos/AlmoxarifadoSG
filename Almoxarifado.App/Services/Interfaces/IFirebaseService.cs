using Almoxarifado.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services.Interfaces
{
    public interface IFirebaseService
    {
        Task<Usuario> GetUsuarioAsync(string uid);
        Task<List<Estoque>> GetEstoqueAsync();
        Task<List<Produto>> GetProdutosAsync();
        Task SalvarProdutoAsync(Produto produto);
    }
}