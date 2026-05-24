namespace Almoxarifado.Domain.Exceptions;
public abstract class DomainException(string message) : Exception(message);
public sealed class EstoqueInsuficienteException(string peca, int solicitado, int disponivel) 
    : DomainException($"Estoque insuficiente para a peça '{peca}'. Solicitado: {solicitado}, Disponível: {disponivel}.");
