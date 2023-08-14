namespace Bazaar.Basket.Core.Usecases;

public abstract class BasketRepositoryResult { }

// Result concrete types
public class BasketSuccessResult :
    BasketRepositoryResult, IAddItemToBasketResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult, ICheckoutResult
{
    public BuyerBasket Basket;

    public BasketSuccessResult(BuyerBasket basket)
    {
        Basket = basket;
    }
}

public class BasketItemSuccessResult :
    BasketRepositoryResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult
{
    public BasketItem BasketItem;

    public BasketItemSuccessResult(BasketItem item)
    {
        BasketItem = item;
    }
}

public class BasketItemNotFoundError :
    BasketRepositoryResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult, ICheckoutResult
{ }

public class BasketItemAlreadyAddedError :
    BasketRepositoryResult, IAddItemToBasketResult
{ }

public class QuantityLessThanOneError :
    BasketRepositoryResult, IChangeItemQuantityResult
{ }

public class ExceptionError :
    BasketRepositoryResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult
{
    public string Error;

    public ExceptionError(string error)
    {
        Error = error;
    }
}

// Result marker interfaces returned by methods
public interface IAddItemToBasketResult
{
    static BasketSuccessResult Success(BuyerBasket b) => new(b);
    static BasketItemAlreadyAddedError BasketItemAlreadyAddedError => new();
}

public interface IChangeItemQuantityResult
{
    static IChangeItemQuantityResult Success(BasketItem item) => new BasketItemSuccessResult(item);
    static IChangeItemQuantityResult QuantityLessThanOneError => new QuantityLessThanOneError();
    static IChangeItemQuantityResult BasketItemNotFoundError => new BasketItemNotFoundError();
    static IChangeItemQuantityResult OtherExceptionError(string error) => new ExceptionError(error);
}

public interface IRemoveItemFromBasketResult
{
    static IRemoveItemFromBasketResult Success(BuyerBasket b) => new BasketSuccessResult(b);
    static IRemoveItemFromBasketResult BasketItemNotFoundError => new BasketItemNotFoundError();
    static IRemoveItemFromBasketResult OtherExceptionError(string error) => new ExceptionError(error);
}

public interface ICheckoutResult
{
    static BasketSuccessResult Success(BuyerBasket b) => new(b);
    static BasketItemNotFoundError BasketHasNoItemsError => new();
}