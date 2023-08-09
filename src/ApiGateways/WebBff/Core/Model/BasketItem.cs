﻿namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class BasketItem
{
    public int Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public uint Quantity { get; set; }
}
