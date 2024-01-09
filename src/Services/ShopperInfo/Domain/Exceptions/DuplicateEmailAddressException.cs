namespace ShopperInfo.Domain.Exceptions;

public class DuplicateEmailAddressException : Exception
{
    public DuplicateEmailAddressException() : base("Email address is taken by another shopper.") { }
}
