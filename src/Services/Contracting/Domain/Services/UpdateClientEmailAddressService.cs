namespace Bazaar.Contracting.Domain.Services;

public class UpdateClientEmailAddressService : IUpdateClientEmailAddressService
{
    private readonly IClientRepository _clientRepo;

    public UpdateClientEmailAddressService(IClientRepository clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public Result UpdateClientEmailAddress(string clientExternalId, string emailAddress)
    {
        var client = _clientRepo.GetWithContractsByExternalId(clientExternalId);
        if (client == null)
        {
            return Result.NotFound("Client not found");
        }

        var emailAddressOwner = _clientRepo
            .GetWithContractsByEmailAddress(emailAddress);
        if (emailAddressOwner != null)
        {
            return emailAddressOwner.Id == client.Id
                ? Result.Success()
                : Result.Conflict(ClientCompliance
                    .UniqueEmailAddressNonComplianceMessage);
        }

        client.ChangeEmailAddress(emailAddress);
        _clientRepo.Update(client);
        // Send confirmation email to new address
        //

        return Result.Success();
    }
}
