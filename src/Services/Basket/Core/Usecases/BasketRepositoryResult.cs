namespace Bazaar.Basket.Core.Usecases;

public abstract class BasketRepositoryResult { }

// Result concrete types
public class BasketSuccessResult :
    BasketRepositoryResult, IAddItemToBasketResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult
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

public class BasketItemNotFoundErrorResult :
    BasketRepositoryResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult
{ }

public class BasketItemAlreadyAddedError :
    BasketRepositoryResult, IAddItemToBasketResult
{ }

public class QuantityLessThanOneErrorResult :
    BasketRepositoryResult, IChangeItemQuantityResult
{ }

public class ExceptionErrorResult :
    BasketRepositoryResult, IChangeItemQuantityResult, IRemoveItemFromBasketResult
{
    public string Error;

    public ExceptionErrorResult(string error)
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
    static IChangeItemQuantityResult QuantityLessThanOneError => new QuantityLessThanOneErrorResult();
    static IChangeItemQuantityResult BasketItemNotFoundError => new BasketItemNotFoundErrorResult();
    static IChangeItemQuantityResult OtherExceptionError(string error) => new ExceptionErrorResult(error);
}

public interface IRemoveItemFromBasketResult
{
    static IRemoveItemFromBasketResult Success(BuyerBasket b) => new BasketSuccessResult(b);
    static IRemoveItemFromBasketResult BasketItemNotFoundError => new BasketItemNotFoundErrorResult();
    static IRemoveItemFromBasketResult OtherExceptionError(string error) => new ExceptionErrorResult(error);
}