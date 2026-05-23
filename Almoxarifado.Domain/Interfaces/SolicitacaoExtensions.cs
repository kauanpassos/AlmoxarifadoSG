using Almoxarifado.Domain;
namespace Almoxarifado.Domain.Interfaces;
public static class SolicitacaoExtensions
{
    public static async Task<IEnumerable<Solicitacao>> GetByUserIdAsync(this IReadOnlyRepository<Solicitacao> repository, string userId)
    {
        var all = await repository.GetAllAsync();
        return all.Where(s => s.UsuarioId == userId);
    }
}
