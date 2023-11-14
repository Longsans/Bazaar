namespace Bazaar.Transport.Domain.Interfaces;

public interface IDeliveryProcessService
{
    Result<Delivery> ScheduleDelivery(int orderId, string deliveryAddress,
        IEnumerable<DeliveryPackageItem> packageItems);
    Result StartDelivery(int deliveryId);
    Result CompleteDelivery(int deliveryId);
    Result PostponeDelivery(int deliveryId);
    Result CancelDelivery(int deliveryId);
}
