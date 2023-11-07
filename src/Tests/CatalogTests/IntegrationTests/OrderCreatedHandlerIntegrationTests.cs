namespace CatalogTests.IntegrationTests;

public class OrderCreatedHandlerIntegrationTests
{
    private readonly BasketCheckoutAcceptedIntegrationEventHandler _handler;
    private readonly CatalogItem _testCatalogItem;

    private readonly EventBusTestDouble _testEventBus;
    private readonly ICatalogRepository _repo;

    public OrderCreatedHandlerIntegrationTests(CatalogDbContext dbContext, EventBusTestDouble testEventBus)
    {

    }
}