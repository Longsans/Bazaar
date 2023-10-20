namespace Bazaar.Contracting.Domain.Exceptions;

public class PartnerAlreadyUnderContractException : Exception
{
    public PartnerAlreadyUnderContractException(string partnerExternalId)
        : base($"Partner with external ID {partnerExternalId} is already under contract") { }

    public PartnerAlreadyUnderContractException(int partnerId)
        : base($"Partner with ID {partnerId} is already under contract") { }
}
