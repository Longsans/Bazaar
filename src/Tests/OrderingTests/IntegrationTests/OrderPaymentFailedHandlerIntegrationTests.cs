namespace OrderingTests.IntegrationTests;

[Collection("OrderingIntegrationTests")]
public class OrderPaymentFailedHandlerIntegrationTests : IDisposable
{
    private readonly OrderPaymentFailedIntegrationEventHandler _handler;
    private readonly OrderingDbContext _dbContext;

    private readonly Order _testOrder;

    public OrderPaymentFailedHandlerIntegrationTests(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.ReseedWithSingleOrder();
        _testOrder = _dbContext.Orders.Single();

        _handler = new(new OrderRepository(_dbContext));
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task Handle_PostponesOrder_WhenOrderExists()
    {
        var paymentFailedEvent = new OrderPaymentFailedIntegrationEvent(_testOrder.Id);

        typeof(Order).GetProperty(nameof(_testOrder.Status))!
            .SetValue(_testOrder, OrderStatus.ProcessingPayment);

        await _handler.Handle(paymentFailedEvent);

        Assert.Equal(OrderStatus.Postponed, _testOrder.Status);
    }

    [Fact]
    public async Task Handle_DoesNothing_WhenOrderDoesNotExist()
    {
        var paymentFailedEvent = new OrderPaymentFailedIntegrationEvent(1000);

        await _handler.Handle(paymentFailedEvent);

        Assert.NotEqual(OrderStatus.Postponed, _testOrder.Status);
    }
}
