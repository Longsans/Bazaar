namespace Bazaar.Basket.Infrastructure.Database;

public class BasketItemChangeTrigger : IBeforeSaveTrigger<BasketItem>
{
    private readonly BasketDbContext _dbContext;

    public BasketItemChangeTrigger(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task BeforeSave(ITriggerContext<BasketItem> context, CancellationToken cancellationToken)
    {
        var basket = context.Entity.Basket
            ?? _dbContext.BuyerBaskets.Find(context.Entity.BasketId);

        if (basket == null)
        {
            return Task.CompletedTask;
        }

        basket.UpdateTotal();
        return Task.CompletedTask;
    }
}
