namespace Almoxarifado.Domain.Exceptions;

public class ForbiddenAccessException(string message) : DomainException(message);
