using Bazaar.Contracting.Infrastructure.Repositories;
using Bazaar.Contracting.ServiceIntegration.IntegrationEvents;

namespace ContractingTests.IntegrationTests;

[Collection(Constants.INTEGRATION_TESTS_COLLECTION)]
public class ProductsHaveFbbStocksHandlerIntegrationTests
{
    private readonly ProductsHaveFbbStocksIntegrationEventHandler _handler;
    private readonly IClientRepository _clientRepo;
    private readonly Client _testClient;

    public ProductsHaveFbbStocksHandlerIntegrationTests(ContractingDbContext dbContext)
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

        _clientRepo = new ClientRepository(dbContext);
        _handler = new(_clientRepo);
    }

    [Fact]
    public async Task Handle_ReopensClientAccout_WhenItIsClosed()
    {
        // arrange
        _testClient.CloseAccount();
        _clientRepo.Update(_testClient);
        var @event = new ProductsHaveFbbStocksIntegrationEvent(
            new string[] { "PROD-1", "PROD-2" }, _testClient.ExternalId);

        // act
        await _handler.Handle(@event);

        // assert
        Assert.False(_testClient.IsAccountClosed);
    }
}
