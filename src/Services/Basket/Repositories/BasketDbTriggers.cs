namespace Bazaar.Basket.Repositories;

public class BasketItemChangeTrigger : IBeforeSaveTrigger<BasketItem>
{
    private readonly BasketDbContext _dbContext;

    public BasketItemChangeTrigger(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task BeforeSave(ITriggerContext<BasketItem> context, CancellationToken cancellationToken)
    {
        var basket = context.Entity.Basket;
        if (basket == null)
        {
            basket = _dbContext.BuyerBaskets.Find(context.Entity.BasketId);
            if (basket == null)
            {
                return Task.CompletedTask;
            }
        }

        basket.Total += context.ChangeType switch
        {
            ChangeType.Deleted => -context.Entity.Quantity * context.Entity.UnitPrice,
            ChangeType.Added => context.Entity.Quantity * context.Entity.UnitPrice,
            ChangeType.Modified => FindItemNewAmount(context.Entity),
            _ => 0
        };

        return Task.CompletedTask;
    }

    private decimal FindItemNewAmount(BasketItem item)
    {
        var entry = _dbContext.Entry(item);
        var originalAmount = entry.Property(e => e.Quantity).OriginalValue * entry.Property(e => e.UnitPrice).OriginalValue;
        var newAmount = entry.Property(e => e.Quantity).CurrentValue * entry.Property(e => e.UnitPrice).CurrentValue;
        return newAmount - originalAmount;
    }
}
