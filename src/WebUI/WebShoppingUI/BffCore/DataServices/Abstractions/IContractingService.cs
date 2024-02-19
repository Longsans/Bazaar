namespace WebShoppingUI.DataServices;

public interface IContractingService
{
    Task<ServiceCallResult<ContractingClient>> GetSellerById(string sellerId);
}
