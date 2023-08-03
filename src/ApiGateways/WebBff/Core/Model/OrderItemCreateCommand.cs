namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class OrderItemCreateCommand
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public int Quantity { get; set; }
}