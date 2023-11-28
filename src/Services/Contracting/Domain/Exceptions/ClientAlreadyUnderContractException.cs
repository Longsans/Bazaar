namespace Bazaar.Contracting.Domain.Exceptions;

public class ClientAlreadyUnderContractException : Exception
{
    public ClientAlreadyUnderContractException(string clientExternalId)
        : base($"Client with external ID {clientExternalId} is already under contract") { }

    public ClientAlreadyUnderContractException(int clientId)
        : base($"Client with ID {clientId} is already under contract") { }
}
