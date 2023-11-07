namespace Bazaar.Contracting.Domain.Exceptions;

public class ClientUnderMinimumAgeException : Exception
{
    public ClientUnderMinimumAgeException(int minimumAge)
        : base($"Client must be {minimumAge} and above.") { }
}
