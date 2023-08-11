namespace Bazaar.Ordering.Repositories;

public class InsertOrderItemsTrigger : IAfterSaveTrigger<OrderItem>
{
    private readonly OrderingDbContext _dbContext;

    public InsertOrderItemsTrigger(OrderingDbContext context)
    {
        _dbContext = context;
    }

    public async Task AfterSave(ITriggerContext<OrderItem> saveContext, CancellationToken cancellationToken)
    {
        var order = _dbContext.Orders.Find(saveContext.Entity.OrderId);

        if (order == null)
        {
            return;
        }

        order.Total += saveContext.ChangeType switch
        {
            ChangeType.Deleted => -(saveContext.Entity.Quantity * saveContext.Entity.ProductUnitPrice),
            ChangeType.Added => saveContext.Entity.Quantity * saveContext.Entity.ProductUnitPrice,
            ChangeType.Modified => FindItemNewAmount(saveContext.Entity),
            _ => 0
        };

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private decimal FindItemNewAmount(OrderItem item)
    {
        var entry = _dbContext.Entry(item);
        var originalAmount = entry.Property(e => e.Quantity).OriginalValue * entry.Property(e => e.ProductUnitPrice).OriginalValue;
        var newAmount = entry.Property(e => e.Quantity).CurrentValue * entry.Property(e => e.ProductUnitPrice).CurrentValue;
        return newAmount - originalAmount;
    }
}
