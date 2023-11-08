﻿namespace Bazaar.Transport.Domain.Entities;

public class DeliveryPackageItem
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public uint Quantity { get; private set; }
    public Delivery Delivery { get; private set; }
    public int DeliveryId { get; private set; }

    public DeliveryPackageItem(string productId, uint quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}
