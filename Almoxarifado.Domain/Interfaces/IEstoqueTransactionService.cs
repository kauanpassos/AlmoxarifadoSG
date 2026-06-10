using System.Threading.Tasks;
using Almoxarifado.Domain.Entities;

namespace Almoxarifado.Domain.Interfaces;

public interface IEstoqueTransactionService
{
    Task<Solicitacao> ProcessarAprovacaoSolicitacaoAsync(string solicitacaoId);
}
