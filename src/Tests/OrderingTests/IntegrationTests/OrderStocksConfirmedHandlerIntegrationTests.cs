﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OrderingTests.IntegrationTests;

[Collection("OrderingIntegrationTests")]
public class OrderStocksConfirmedHandlerIntegrationTests : IDisposable
{
    private readonly OrderItemsStockConfirmedIntegrationEventHandler _handler;
    private readonly EventBusTestDouble _testEventBus;
    private readonly OrderingDbContext _dbContext;

    public OrderStocksConfirmedHandlerIntegrationTests(
        OrderingDbContext dbContext, EventBusTestDouble testEventBus,
        ILogger<OrderItemsStockConfirmedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext.ReseedWithSingleOrder();

        _testEventBus = testEventBus;
        var orderRepo = new OrderRepository(_dbContext);
        var handleService = new HandleOrderService(orderRepo, _testEventBus);
        _handler = new(orderRepo, handleService, logger);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task Handle_StartsOrderPaymentAndPublishesStatusEvent_WhenOrderExists()
    {
        var testOrder = _dbContext.Orders.Single();
        var stocksConfirmedEvent = new OrderItemsStockConfirmedIntegrationEvent(
            testOrder.Id, testOrder.Items.Select(x => x.ProductId));

        await _handler.Handle(stocksConfirmedEvent);

        var orderEntry = _dbContext.Entry(testOrder);
        Assert.Equal(OrderStatus.ProcessingPayment, testOrder.Status);
        Assert.Equal(EntityState.Unchanged, orderEntry.State);

        var publishedEvent = _testEventBus.GetEvent<OrderStatusChangedToProcessingPaymentIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(testOrder.Id, publishedEvent.OrderId);
    }

    [Fact]
    public async Task Handle_DoesNothing_WhenOrderNotExist()
    {
        var testOrder = _dbContext.Orders.Single();
        var stocksConfirmedEvent = new OrderItemsStockConfirmedIntegrationEvent(
            1000, Array.Empty<string>());

        await _handler.Handle(stocksConfirmedEvent);

        var orderEntry = _dbContext.Entry(testOrder);
        Assert.NotEqual(OrderStatus.ProcessingPayment, testOrder.Status);
        Assert.Equal(EntityState.Unchanged, orderEntry.State);

        var publishedEvent = _testEventBus.GetEvent<OrderStatusChangedToProcessingPaymentIntegrationEvent>();
        Assert.Null(publishedEvent);
    }
}
