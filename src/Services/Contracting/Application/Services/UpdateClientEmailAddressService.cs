namespace Bazaar.Contracting.Application.Services;

public class UpdateClientEmailAddressService
{
    private readonly IRepository<Client> _clientRepo;

    public UpdateClientEmailAddressService(IRepository<Client> clientRepo)
    {
        _clientRepo = clientRepo;
    }

    public async Task<Result> UpdateClientEmailAddress(string clientExternalId, string emailAddress)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(
            new ClientByExternalIdSpec(clientExternalId));
        if (client == null)
        {
            return Result.NotFound("Client not found");
        }

        var emailAddressOwner = await _clientRepo.SingleOrDefaultAsync(
            new ClientByEmailAddressSpec(emailAddress));
        if (emailAddressOwner != null)
        {
            return emailAddressOwner.Id == client.Id
                ? Result.Success()
                : Result.Conflict(ClientCompliance
                    .UniqueEmailAddressNonComplianceMessage);
        }

        client.ChangeEmailAddress(emailAddress);
        await _clientRepo.UpdateAsync(client);
        // Send confirmation email to new address
        //

        return Result.Success();
    }
}
