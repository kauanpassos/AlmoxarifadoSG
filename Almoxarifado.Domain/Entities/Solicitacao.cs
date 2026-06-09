using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace Almoxarifado.Domain.Entities;

[FirestoreData]
public sealed class Solicitacao
{
    [FirestoreProperty]
    public string Id { get; private set; }

    [FirestoreProperty]
    public string UsuarioId { get; private set; }

    [FirestoreProperty]
    public string Observacao { get; private set; }

    [FirestoreProperty]
    public string Status { get; private set; }

    private readonly List<ItemSolicitacao> _itens = new();

    // Mantemos a coleção de leitura segura para a aplicação
    public IReadOnlyCollection<ItemSolicitacao> Itens => _itens.AsReadOnly();

    // Propriedade exclusiva para o Firestore gravar/ler a lista sem quebrar o encapsulamento
    [FirestoreProperty("Itens")]
    public List<ItemSolicitacao> ItensDb
    {
        get => _itens;
        set
        {
            _itens.Clear();
            if (value != null) _itens.AddRange(value);
        }
    }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    [FirestoreProperty]
    public DateTime UpdatedAt { get; private set; }

    // Construtor vazio exigido pelo SDK do Firestore
    public Solicitacao() { }

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

    // 🔥 CORREÇÃO: Método Cancelar adicionado para que o Handler possa chamá-lo
    public void Cancelar()
    {
        if (Status != "Pendente" && Status != "Em análise")
            throw new InvalidOperationException("Apenas solicitações pendentes ou em análise podem ser canceladas.");

        Status = "Cancelado";
        AtualizarData(); // Atualiza a data de modificação automaticamente
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}