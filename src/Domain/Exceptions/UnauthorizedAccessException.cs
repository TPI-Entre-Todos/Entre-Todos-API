namespace Domain.Exceptions;

public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string message) : base(message)
    {
    }

    public UnauthorizedAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UnauthorizedAccessException() : base("No tienes permiso para acceder a este recurso.")
    {
    }
}
