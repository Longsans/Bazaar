﻿namespace Bazaar.Basket.Web.Messages;

public class AddBasketItemRequest
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public uint Quantity { get; set; }
    public string ImageUrl { get; set; }
}