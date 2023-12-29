namespace Bazaar.Basket.Application.DTOs;

public class BasketCheckout
{
    public string BuyerId { get; }

    public string ShippingAddress { get; }
    public string City { get; }
    public string Country { get; }
    public string ZipCode { get; }

    public BasketCheckout(string buyerId, string shippingAddress, string city, string country, string zipCode)
    {
        BuyerId = buyerId;
        ShippingAddress = shippingAddress;
        City = city;
        Country = country;
        ZipCode = zipCode;
    }
}
