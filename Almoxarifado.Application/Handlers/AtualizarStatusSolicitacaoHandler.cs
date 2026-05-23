using MediatR;
using Almoxarifado.Domain;
using Almoxarifado.Domain.Interfaces;
using Almoxarifado.Application.Commands;
namespace Almoxarifado.Application.Handlers;
public sealed class AtualizarStatusSolicitacaoHandler(
    IReadOnlyRepository<Solicitacao> readRepository,
    IWriteOnlyRepository<Solicitacao> writeRepository)
    : IRequestHandler<AtualizarStatusSolicitacaoCommand, Solicitacao>
{
    public async Task<Solicitacao> Handle(AtualizarStatusSolicitacaoCommand request, CancellationToken cancellationToken)
    {
        var solicitacao = await readRepository.GetByIdAsync(request.SolicitacaoId)
            ?? throw new KeyNotFoundException($"Solicitação '{request.SolicitacaoId}' não encontrada.");
        switch (request.NovoStatus)
        {
            case "Aprovada":
                solicitacao.Aprovar();
                break;
            case "Recusada":
                solicitacao.Recusar();
                break;
            case "Entregue":
                solicitacao.FinalizarEntrega();
                break;
            default:
                throw new ArgumentException($"Status '{request.NovoStatus}' inválido. Use: Aprovada, Recusada ou Entregue.");
        }
        await writeRepository.UpdateAsync(solicitacao);
        return solicitacao;
    }
}
