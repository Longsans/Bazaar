namespace Bazaar.ApiGateways.WebBff.DTOs;

public record BasketItemAddCommand
(
    string ProductId,
    uint Quantity
);