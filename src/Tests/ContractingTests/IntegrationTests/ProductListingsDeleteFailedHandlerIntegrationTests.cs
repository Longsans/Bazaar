namespace ContractingTests.IntegrationTests;

[Collection(Constants.INTEGRATION_TESTS_COLLECTION)]
public class ProductListingsDeleteFailedHandlerIntegrationTests
{
    private readonly ProductListingsDeleteFailedIntegrationEventHandler _handler;
    private readonly IRepository<Client> _clientRepo;
    private readonly Client _testClient;

    public ProductListingsDeleteFailedHandlerIntegrationTests(
        ContractingDbContext dbContext, IRepository<Client> clientRepo,
        ILogger<ProductListingsDeleteFailedIntegrationEventHandler> logger)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        var testPlan = new SellingPlan("Test", 10m, 0.99m, 0.01f);
        dbContext.SellingPlans.Add(testPlan);
        dbContext.SaveChanges();

        _testClient = new Client(
            "Test", "Test", "test@testmail.com", "0901234567",
            new DateTime(1989, 11, 11), Gender.Male, 1);
        dbContext.Clients.Add(_testClient);
        dbContext.SaveChanges();

        _clientRepo = clientRepo;
        _handler = new(_clientRepo, logger);
    }

    [Fact]
    public async Task Handle_ReopensClientAccout_WhenItIsClosed()
    {
        // arrange
        _testClient.CloseAccount();
        await _clientRepo.UpdateAsync(_testClient);
        var failedListings = new FailedDeleteListing[]
        {
            new("PROD-1", ListingDeleteFailureReason.HasOrdersInProgress),
            new("PROD-2", ListingDeleteFailureReason.HasFbbStock),
        };
        var @event = new ProductListingsDeleteFailedIntegrationEvent(
            failedListings, _testClient.ExternalId);

        // act
        await _handler.Handle(@event);

        // assert
        Assert.False(_testClient.IsAccountClosed);
    }
}
