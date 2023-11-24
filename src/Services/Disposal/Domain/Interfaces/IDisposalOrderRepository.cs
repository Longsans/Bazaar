namespace Bazaar.Disposal.Domain.Interfaces;

public interface IDisposalOrderRepository
{
    DisposalOrder? GetById(int id);
    IEnumerable<DisposalOrder> GetByInventoryOwnerId(string inventoryOwnerId);
    DisposalOrder Create(DisposalOrder disposalOrder);
    void Update(DisposalOrder disposalOrder);
}
