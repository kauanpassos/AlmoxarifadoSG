using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;
using Almoxarifado.Domain.Constants;

namespace Almoxarifado.Domain.Entities;

[FirestoreData]
public sealed class Solicitacao
{
    [FirestoreDocumentId]
    public string Id { get; set; } = null!;

    [FirestoreProperty("UsuarioId")]
    public string UsuarioId { get; set; } = null!;

    [FirestoreProperty("Observacao")]
    public string Observacao { get; set; } = null!;

    [FirestoreProperty("Status")]
    public string Status { get; set; } = null!;

    private readonly List<ItemSolicitacao> _itens = new();

    public IReadOnlyCollection<ItemSolicitacao> Itens => _itens.AsReadOnly();

    [FirestoreProperty("Itens")]
    public List<ItemSolicitacao> ItensDb
    {
        get => _itens;
        set
        {
            _itens.Clear();

            if (value != null)
                _itens.AddRange(value);
        }
    }

    [FirestoreProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [FirestoreProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    public Solicitacao() { }

    public Solicitacao(string id, string usuarioId, string observacao)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);

        Id = id;
        UsuarioId = usuarioId;
        Observacao = string.IsNullOrWhiteSpace(observacao)
            ? "Sem observações"
            : observacao.Trim();

        Status = StatusSolicitacao.Pendente;
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
        if (Status != StatusSolicitacao.Pendente)
            throw new InvalidOperationException("Apenas solicitações em estado pendente podem ser aprovadas.");

        Status = StatusSolicitacao.Aprovada;
        AtualizarData();
    }

    public void Recusar()
    {
        if (Status != StatusSolicitacao.Pendente)
            throw new InvalidOperationException("Apenas solicitações em estado pendente podem ser recusadas.");

        Status = StatusSolicitacao.Recusada;
        AtualizarData();
    }

    public void FinalizarEntrega()
    {
        if (Status != StatusSolicitacao.Aprovada)
            throw new InvalidOperationException("Apenas solicitações previamente aprovadas podem ser entregues.");

        Status = StatusSolicitacao.Entregue;
        AtualizarData();
    }

    public void Cancelar()
    {
        if (Status != StatusSolicitacao.Pendente &&
            Status != StatusSolicitacao.EmAnalise)
        {
            throw new InvalidOperationException("Apenas solicitações pendentes ou em análise podem ser canceladas.");
        }

        Status = StatusSolicitacao.Cancelada;
        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}