namespace Bazaar.Ordering.Infrastructure.Database;

public class AddOrderTrigger : IBeforeSaveTrigger<Order>
{
    private readonly OrderingDbContext _dbContext;

    public AddOrderTrigger(OrderingDbContext context)
    {
        _dbContext = context;
    }

    public async Task BeforeSave(ITriggerContext<Order> saveContext, CancellationToken cancellationToken)
    {
        if (saveContext.ChangeType != ChangeType.Added)
            return;

        saveContext.Entity.UpdateTotal();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
