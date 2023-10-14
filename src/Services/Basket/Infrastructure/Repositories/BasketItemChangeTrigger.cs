namespace Bazaar.Basket.Infrastructure.Repositories;

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

        var newTotal = basket.Total + context.ChangeType switch
        {
            ChangeType.Added => context.Entity.Quantity * context.Entity.UnitPrice,
            ChangeType.Deleted => CalculateAmountDifference(context.Entity),
            ChangeType.Modified => CalculateAmountDifference(context.Entity),
            _ => 0
        };

        basket.UpdateTotal(newTotal);
        return Task.CompletedTask;
    }

    private decimal CalculateAmountDifference(BasketItem item)
    {
        var entry = _dbContext.Entry(item);

        var originalAmount = entry.Property(e => e.Quantity).OriginalValue
            * entry.Property(e => e.UnitPrice).OriginalValue;

        var newAmount = entry.Property(e => e.Quantity).CurrentValue
            * entry.Property(e => e.UnitPrice).CurrentValue;

        return newAmount - originalAmount;
    }
}
