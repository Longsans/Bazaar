namespace Bazaar.Basket.Domain.Entites;

public class BasketCheckout
{
    public string BuyerId { get; init; }

    //public string City { get; init; }
    //public string Country { get; init; }
    //public string ZipCode { get; init; }
    //public string ShippingAddress { get; init; }

    //public string CardNumber { get; init; }
    //public string CardHolderName { get; init; }
    //public DateTime CardExpiration { get; init; }
    //public string CardSecurityNumber { get; init; }

    public BasketCheckout(string buyerId)
    //string city, string country, string zipCode, string shippingAddress,
    //string cardNumber, string cardHolderName, DateTime cardExpiration, string cardSecurityNumber)
    {
        BuyerId = buyerId;
        //City = city;
        //Country = country;
        //ZipCode = zipCode;
        //ShippingAddress = shippingAddress;
        //CardNumber = cardNumber;
        //CardHolderName = cardHolderName;
        //CardExpiration = cardExpiration;
        //CardSecurityNumber = cardSecurityNumber;
    }
}
