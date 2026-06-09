using Almoxarifado.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Almoxarifado.App.Services.Interfaces;

public interface IFirebaseService
{
    Task<UsuarioDto> GetUsuarioAsync(string uid);
    Task<IEnumerable<ProdutoDto>> GetProdutosAsync();
    Task SalvarProdutoAsync(ProdutoDto produto);
    Task EnviarSolicitacaoLoteAsync(string usuarioId, string observacao, IEnumerable<CartItemDto> itens);
    Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesUsuarioAsync(string usuarioId);
    Task<IEnumerable<SolicitacaoDto>> GetSolicitacoesPendentesAsync();
    Task AtualizarStatusSolicitacaoAsync(string id, string novoStatus);
}