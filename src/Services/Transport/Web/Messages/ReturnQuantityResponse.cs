namespace Bazaar.Transport.Web.Messages;

public record ReturnQuantityResponse(
    int Id,
    string LotNumber,
    uint Units,
    int ReturnId)
{
    public ReturnQuantityResponse(ReturnQuantity returnQty)
        : this(returnQty.Id, returnQty.LotNumber, returnQty.Units, returnQty.ReturnId)
    {

    }
}
