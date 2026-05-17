using System;

namespace Almoxarifado.Domain;

public sealed class Solicitacao
{
    public string Id { get; }
    public string UsuarioId { get; }
    public string Observacao { get; private set; }
    public string Status { get; private set; }
    public string? Sku { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public Solicitacao(string id, string usuarioId, string observacao)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O ID da solicitação é obrigatório.", nameof(id));

        if (string.IsNullOrWhiteSpace(usuarioId))
            throw new ArgumentException("O ID do usuário (solicitante) é obrigatório.", nameof(usuarioId));

        Id = id;
        UsuarioId = usuarioId;
        Observacao = string.IsNullOrWhiteSpace(observacao) ? "Sem observações" : observacao.Trim();
        Status = "Pendente";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Aprovar()
    {
        if (Status != "Pendente")
            throw new InvalidOperationException("Apenas solicitações pendentes podem ser aprovadas.");

        Status = "Aprovada";
        AtualizarData();
    }
    public void Recusar()
    {
        if (Status != "Pendente")
            throw new InvalidOperationException("Apenas solicitações pendentes podem ser recusadas.");

        Status = "Recusada";
        AtualizarData();
    }
    public void FinalizarEntrega()
    {
        if (Status != "Aprovada")
            throw new InvalidOperationException("Apenas solicitações aprovadas podem ser entregues.");

        Status = "Entregue";
        AtualizarData();
    }

    private void AtualizarData()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}