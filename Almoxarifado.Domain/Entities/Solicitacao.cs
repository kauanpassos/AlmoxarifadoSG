using System;

namespace Almoxarifado.Domain.Entities;

public sealed class Solicitacao
{
    public string Id { get; }
    public string UsuarioId { get; }
    public string Observacao { get; private set; }
    public string Status { get; private set; }
    private readonly List<ItemSolicitacao> _itens = new();
    public IReadOnlyCollection<ItemSolicitacao> Itens => _itens.AsReadOnly();

    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public Solicitacao(string id, string usuarioId, string observacao)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);

        Id = id;
        UsuarioId = usuarioId;
        Observacao = string.IsNullOrWhiteSpace(observacao) ? "Sem observações" : observacao.Trim();
        Status = "Pendente";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdicionarItem(ItemSolicitacao item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        _itens.Add(item);
        AtualizarData();
    }
    public void Aprovar()
    {
        if (Status is not "Pendente")
            throw new InvalidOperationException("Apenas solicitações pendentes podem ser aprovadas.");

        Status = "Aprovada";
        AtualizarData();
    }
    public void Recusar()
    {
        if (Status is not "Pendente")
            throw new InvalidOperationException("Apenas solicitações pendentes podem ser recusadas.");

        Status = "Recusada";
        AtualizarData();
    }
    public void FinalizarEntrega()
    {
        if (Status is not "Aprovada")
            throw new InvalidOperationException("Apenas solicitações aprovadas podem ser entregues.");

        Status = "Entregue";
        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}