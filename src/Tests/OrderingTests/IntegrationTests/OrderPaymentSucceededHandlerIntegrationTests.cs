using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OrderingTests.IntegrationTests;

[Collection("OrderingIntegrationTests")]
public class OrderPaymentSucceededHandlerIntegrationTests : IDisposable
{
    private readonly OrderPaymentSucceededIntegrationEventHandler _handler;
    private readonly OrderingDbContext _dbContext;

    public OrderPaymentSucceededHandlerIntegrationTests(OrderingDbContext dbContext,
        ILogger<OrderPaymentSucceededIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _dbContext.ReseedWithSingleOrder();

        _handler = new(new OrderRepository(_dbContext), logger);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task Handle_PutsOrderOnWaitForSellerConfirmation_WhenOrderExists()
    {
        var testOrder = _dbContext.Orders.Single();
        var paymentSuccessEvent = new OrderPaymentSucceededIntegrationEvent(testOrder.Id);
        typeof(Order).GetProperty(nameof(testOrder.Status))!
            .SetValue(testOrder, OrderStatus.ProcessingPayment);

        await _handler.Handle(paymentSuccessEvent);

        var orderEntry = _dbContext.Entry(testOrder);
        Assert.Equal(OrderStatus.AwaitingSellerConfirmation, testOrder.Status);
        Assert.Equal(EntityState.Unchanged, orderEntry.State);
    }

    [Fact]
    public async Task Handle_DoesNothing_WhenOrderDoesNotExist()
    {
        var testOrder = _dbContext.Orders.Single();
        var paymentSuccessEvent = new OrderPaymentSucceededIntegrationEvent(1000);

        await _handler.Handle(paymentSuccessEvent);

        Assert.NotEqual(OrderStatus.AwaitingSellerConfirmation, testOrder.Status);
    }
}
