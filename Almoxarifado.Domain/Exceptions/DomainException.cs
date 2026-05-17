namespace Almoxarifado.Domain.Exceptions;

// Classe base para todas as exceções de regra de negócio do sistema.
public abstract class DomainException(string message) : Exception(message);

// Exemplo real: Lançada quando o usuário tenta pedir mais do que há no estoque.
public sealed class EstoqueInsuficienteException(string peca, int solicitado, int disponivel)
    : DomainException($"Estoque insuficiente para a peça '{peca}'. Solicitado: {solicitado}, Disponível: {disponivel}.");
