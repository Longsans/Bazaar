namespace Bazaar.Transport.Domain.Interfaces;

public interface IDeliveryRepository
{
    Delivery? GetById(int id);
    IEnumerable<Delivery> GetIncomplete();
    Delivery Create(Delivery delivery);
    void Update(Delivery delivery);
}
