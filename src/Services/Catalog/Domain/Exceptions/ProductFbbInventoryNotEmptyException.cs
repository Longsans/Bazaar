namespace Bazaar.Catalog.Domain.Exceptions;

public class ProductFbbInventoryNotEmptyException : Exception
{
    public ProductFbbInventoryNotEmptyException()
        : base("Cannot delete a FBB product until its FBB inventory has been emptied.") { }
}
