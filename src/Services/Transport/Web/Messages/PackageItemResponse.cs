namespace Bazaar.Transport.Web.Messages;

public class PackageItemResponse
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public uint Quantity { get; private set; }

    public PackageItemResponse(DeliveryPackageItem item)
    {
        Id = item.Id;
        ProductId = item.ProductId;
        Quantity = item.Quantity;
    }
}
