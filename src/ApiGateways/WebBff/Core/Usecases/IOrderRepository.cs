namespace Bazaar.ApiGateways.WebBff.Core.Usecases;

public interface IOrderRepository
{
    Task<Order> Create(Order order);
}
