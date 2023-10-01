namespace WebShoppingUI.DTOs;

public record BasketItemAddCommand
(
    string ProductId,
    uint Quantity
);