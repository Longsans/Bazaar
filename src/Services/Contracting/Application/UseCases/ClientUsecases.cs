namespace Bazaar.Contracting.Application;

public class ClientUseCases : IClientUseCases
{
    private readonly IClientRepository _clientRepo;
    private readonly IUpdateClientEmailAddressService _updateEmailAddressService;

    public ClientUseCases(
        IClientRepository clientRepo,
        IUpdateClientEmailAddressService updateEmailService)
    {
        _clientRepo = clientRepo;
        _updateEmailAddressService = updateEmailService;
    }

    public Client? GetWithContractsById(int clientId)
    {
        return _clientRepo.GetWithContractsById(clientId);
    }

    public Client? GetWithContractsByExternalId(string clientExternalId)
    {
        return _clientRepo.GetWithContractsByExternalId(clientExternalId);
    }

    public Result<ClientDto> RegisterClient(ClientDto clientDto)
    {
        Client client;
        try
        {
            client = clientDto.ToNewClient();
        }
        catch (ClientUnderMinimumAgeException)
        {
            return ClientUnderMinimumAge(nameof(ClientDto.DateOfBirth));
        }

        var existingEmailAddressOwner = _clientRepo
            .GetWithContractsByEmailAddress(clientDto.EmailAddress);

        if (existingEmailAddressOwner is not null)
        {
            return Result.Conflict(
                ClientCompliance.UniqueEmailAddressNonComplianceMessage);
        }

        _clientRepo.Create(client);
        return Result.Success(new ClientDto(client));
    }

    public Result UpdateClientInfoByExternalId(ClientDto clientDto)
    {
        var client = _clientRepo
            .GetWithContractsByExternalId(clientDto.ExternalId!);
        if (client == null)
        {
            return Result.NotFound();
        }

        Client updatedClient;
        clientDto.EmailAddress = client.EmailAddress;
        try
        {
            updatedClient = clientDto.ToExistingClient(client.Id, client.ExternalId);
        }
        catch (ClientUnderMinimumAgeException)
        {
            return ClientUnderMinimumAge(nameof(ClientDto.DateOfBirth));
        }

        _clientRepo.Update(updatedClient);
        return Result.Success();
    }

    public Result UpdateClientEmailAddress(string clientExternalId, string newEmail)
    {
        return _updateEmailAddressService
            .UpdateClientEmailAddress(clientExternalId, newEmail);
    }

    private static Result ClientUnderMinimumAge(string dobPropName)
    {
        return Result.Invalid(new()
        {
            new()
            {
                Identifier = dobPropName,
                ErrorMessage = ClientCompliance.ClientMinimumAgeStatement
            }
        });
    }
}
