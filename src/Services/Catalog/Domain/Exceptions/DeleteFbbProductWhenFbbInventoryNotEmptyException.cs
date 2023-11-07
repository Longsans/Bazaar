namespace Bazaar.Catalog.Domain.Exceptions;

public class DeleteFbbProductWhenFbbInventoryNotEmptyException : Exception
{
    public DeleteFbbProductWhenFbbInventoryNotEmptyException()
        : base("Cannot delete a FBB product until its FBB inventory has been emptied.") { }
}
