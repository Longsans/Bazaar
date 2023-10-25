using Bazaar.Contracting.Infrastructure.Repositories;

namespace ContractingTests.IntegrationTests;

public class UpdateEmailAddressServiceIntegrationTests
{
    private readonly IClientRepository _clientRepo;
    private readonly UpdateClientEmailAddressService _service;
    private readonly Client _testClient;

    public UpdateEmailAddressServiceIntegrationTests(ContractingDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        _clientRepo = new ClientRepository(dbContext);
        _service = new UpdateClientEmailAddressService(_clientRepo);

        _testClient = new("Test", "Test",
            "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male);
        _clientRepo.Create(_testClient);
    }

    [Fact]
    public void UpdateClientEmailAddress_Succeeds_WhenEmailAddressNotExistYet()
    {
        var newEmailAddress = "newmail@testmail.com";

        var result = _service.UpdateClientEmailAddress(
            _testClient.ExternalId, newEmailAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmailAddress, _testClient.EmailAddress);

        var emailAddressOwner = _clientRepo
            .GetWithContractsByEmailAddress(newEmailAddress);

        Assert.NotNull(emailAddressOwner);
        Assert.Equal(emailAddressOwner.Id, _testClient.Id);
    }

    [Fact]
    public void UpdateClientEmailAddress_Succeeds_WhenEmailAddressDoesNotChange()
    {
        var originalEmailAddress = _testClient.EmailAddress;

        var result = _service.UpdateClientEmailAddress(
            _testClient.ExternalId, originalEmailAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalEmailAddress, _testClient.EmailAddress);

        var emailAddressOwner = _clientRepo
            .GetWithContractsByEmailAddress(originalEmailAddress);

        Assert.NotNull(emailAddressOwner);
        Assert.Equal(emailAddressOwner.Id, _testClient.Id);
    }

    [Fact]
    public void UpdateClientEmailAddress_ReturnsNotFound_WhenClientNotFound()
    {
        var clientExternalId = "CLNT-1000";
        var newEmailAddress = "newmail@testmail.com";

        var result = _service.UpdateClientEmailAddress(
            clientExternalId, newEmailAddress);

        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.NotEqual(newEmailAddress, _testClient.EmailAddress);

        var newEmailAddressOwner = _clientRepo
            .GetWithContractsByEmailAddress(newEmailAddress);

        Assert.Null(newEmailAddressOwner);
    }

    [Fact]
    public void UpdateClientEmailAddress_ReturnsConflict_WhenEmailAddressAlreadyExists()
    {
        // arrange
        var existingEmailAddress = "existing@testmail.com";
        var existingOwner = _testClient.WithDifferentId(2, "CLNT-2")
            .WithEmailAddress(existingEmailAddress);

        _clientRepo.Create(existingOwner);

        // act
        var result = _service.UpdateClientEmailAddress(
            _testClient.ExternalId, existingEmailAddress);

        // assert
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.NotEqual(existingEmailAddress, _testClient.EmailAddress);

        var emailAddressOwner = _clientRepo
            .GetWithContractsByEmailAddress(existingEmailAddress);

        Assert.NotNull(emailAddressOwner);
        Assert.NotEqual(emailAddressOwner.Id, _testClient.Id);
    }
}
