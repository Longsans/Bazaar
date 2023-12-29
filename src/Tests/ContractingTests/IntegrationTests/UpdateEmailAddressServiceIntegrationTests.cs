namespace ContractingTests.IntegrationTests;

[Collection(Constants.INTEGRATION_TESTS_COLLECTION)]
[CollectionDefinition(Constants.INTEGRATION_TESTS_COLLECTION, DisableParallelization = true)]
public class UpdateEmailAddressServiceIntegrationTests
{
    private readonly IRepository<Client> _clientRepo;
    private readonly UpdateClientEmailAddressService _service;
    private readonly Client _testClient;
    private readonly ContractingDbContext _dbContext;

    private Client? GetEmailAddressOwnerFromDb(string emailAddress)
    {
        return _dbContext.Clients.SingleOrDefault(x => x.EmailAddress == emailAddress);
    }

    public UpdateEmailAddressServiceIntegrationTests(
        ContractingDbContext dbContext, IRepository<Client> clientRepo)
    {
        _dbContext = dbContext;
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        _clientRepo = clientRepo;
        _service = new UpdateClientEmailAddressService(_clientRepo);

        var sellingPlan = new SellingPlan("Individual", 0m, 1.99m, 0.05f);
        dbContext.SellingPlans.Add(sellingPlan);
        dbContext.SaveChanges();

        _testClient = new("Test", "Test",
            "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male, sellingPlan.Id);
        dbContext.Clients.Add(_testClient);
        dbContext.SaveChanges();
    }

    [Fact]
    public async Task UpdateClientEmailAddress_Succeeds_WhenEmailAddressNotExistYet()
    {
        var newEmailAddress = "newmail@testmail.com";

        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, newEmailAddress);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmailAddress, _testClient.EmailAddress);
        var emailAddressOwner = GetEmailAddressOwnerFromDb(newEmailAddress);
        Assert.Equal(emailAddressOwner, _testClient);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_Succeeds_WhenEmailAddressDoesNotChange()
    {
        var originalEmailAddress = _testClient.EmailAddress;

        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, originalEmailAddress);

        Assert.True(result.IsSuccess);
        var emailAddressOwner = GetEmailAddressOwnerFromDb(originalEmailAddress);
        Assert.Equal(emailAddressOwner, _testClient);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_ReturnsNotFound_WhenClientNotFound()
    {
        var clientExternalId = "CLNT-1000";
        var newEmailAddress = "newmail@testmail.com";

        var result = await _service.UpdateClientEmailAddress(
            clientExternalId, newEmailAddress);

        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.NotEqual(newEmailAddress, _testClient.EmailAddress);
        var newEmailAddressOwner = GetEmailAddressOwnerFromDb(newEmailAddress);
        Assert.Null(newEmailAddressOwner);
    }

    [Fact]
    public async Task UpdateClientEmailAddress_ReturnsConflict_WhenEmailAddressAlreadyExists()
    {
        // arrange
        var existingEmailAddress = "existing@testmail.com";
        var existingOwner = _testClient.WithEmailAddress(existingEmailAddress);
        _dbContext.Clients.Add(existingOwner);
        _dbContext.SaveChanges();

        // act
        var result = await _service.UpdateClientEmailAddress(
            _testClient.ExternalId, existingEmailAddress);

        // assert
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.NotEqual(existingEmailAddress, _testClient.EmailAddress);
        var emailAddressOwner = GetEmailAddressOwnerFromDb(existingEmailAddress);
        Assert.NotNull(emailAddressOwner);
        Assert.NotEqual(emailAddressOwner.Id, _testClient.Id);
    }
}
