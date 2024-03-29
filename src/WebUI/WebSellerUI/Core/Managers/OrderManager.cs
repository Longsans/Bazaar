﻿namespace WebSellerUI.Managers;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;

public class OrderManager
{
    private readonly IOrderingDataService _orderingService;
    private readonly ICatalogDataService _catalogService;

    public OrderManager(IOrderingDataService orderingService, ICatalogDataService catalogService)
    {
        _orderingService = orderingService;
        _catalogService = catalogService;
    }

    public async Task<OrderCollectionResult> GetBySellerId(string sellerId)
    {
        var catalogResult = await _catalogService.GetBySellerId(sellerId);
        if (!catalogResult.IsSuccess)
            return new OrderCollectionResult(catalogResult.ErrorType, catalogResult.ErrorDetail);

        var joinedProductIds = string.Join(',', catalogResult.Result!.Select(item => item.ProductId));
        var ordersResult = await _orderingService.GetByProductIds(joinedProductIds);
        return ordersResult;
    }

    public async Task<ServiceCallResult> ConfirmOrder(int orderId)
    {
        var updateCommand = new OrderUpdateStatusCommand
        {
            Status = OrderStatus.Shipping,
        };
        return await _orderingService.UpdateStatus(orderId, updateCommand);
    }

    public async Task<ServiceCallResult> CancelOrder(int orderId, string reason)
    {
        var updateCommand = new OrderUpdateStatusCommand
        {
            Status = OrderStatus.Cancelled,
            CancelReason = reason
        };
        return await _orderingService.UpdateStatus(orderId, updateCommand);
    }
}
