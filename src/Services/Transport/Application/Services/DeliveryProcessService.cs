namespace Bazaar.Transport.Domain.Services;

public class DeliveryProcessService
{
    private readonly IDeliveryRepository _deliveryRepo;
    private readonly IEstimationService _estimationService;
    private readonly IEventBus _eventBus;

    public DeliveryProcessService(
        IDeliveryRepository deliveryRepo,
        IEstimationService estimationService,
        IEventBus eventBus)
    {
        _deliveryRepo = deliveryRepo;
        _estimationService = estimationService;
        _eventBus = eventBus;
    }

    public Result<Delivery> ScheduleDelivery(int orderId, string deliveryAddress,
        IEnumerable<DeliveryPackageItem> packageItems)
    {
        var estimatedDeliveryTime = _estimationService
            .EstimateDeliveryCompletion(packageItems);
        try
        {
            var delivery = new Delivery(orderId, deliveryAddress,
                packageItems, estimatedDeliveryTime);
            _deliveryRepo.Create(delivery);
            PublishStatusChangedEvent(delivery);

            return Result.Success(delivery);
        }
        catch (ArgumentException ex)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = ex.ParamName,
                ErrorMessage = ex.Message,
            });
        }
    }

    public Result StartDelivery(int deliveryId)
    {
        return FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.StartDelivery();
        });
    }

    public Result CompleteDelivery(int deliveryId)
    {
        return FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.Complete();
        }, PublishStatusChangedEvent);
    }

    public Result PostponeDelivery(int deliveryId)
    {
        return FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.Postpone();
        }, PublishStatusChangedEvent);
    }

    public Result CancelDelivery(int deliveryId)
    {
        return FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.Cancel();
        }, PublishStatusChangedEvent);
    }

    #region Helpers
    private Result FindAndUpdateStatus(int deliveryId,
        Action<Delivery> statusChange, Action<Delivery>? afterUpdateDo = null)
    {
        var delivery = _deliveryRepo.GetById(deliveryId);
        if (delivery == null)
        {
            return Result.NotFound();
        }

        try
        {
            statusChange(delivery);
            _deliveryRepo.Update(delivery);
            afterUpdateDo?.Invoke(delivery);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }
    }

    private void PublishStatusChangedEvent(Delivery delivery)
    {
        _eventBus.Publish(new DeliveryStatusChangedIntegrationEvent(
                delivery.Id, delivery.OrderId, delivery.Status));
    }
    #endregion
}
