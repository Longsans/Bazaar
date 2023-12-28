namespace Bazaar.Transport.Domain.Services;

public class DeliveryProcessService
{
    private readonly Repository<Delivery> _deliveryRepo;
    private readonly IEstimationService _estimationService;
    private readonly IEventBus _eventBus;

    public DeliveryProcessService(
        Repository<Delivery> deliveryRepo,
        IEstimationService estimationService,
        IEventBus eventBus)
    {
        _deliveryRepo = deliveryRepo;
        _estimationService = estimationService;
        _eventBus = eventBus;
    }

    public async Task<Result<Delivery>> ScheduleDelivery(int orderId, string deliveryAddress,
        IEnumerable<DeliveryPackageItem> packageItems)
    {
        var estimatedDeliveryTime = await _estimationService
            .EstimateDeliveryCompletion(packageItems);
        try
        {
            var delivery = new Delivery(orderId, deliveryAddress,
                packageItems, estimatedDeliveryTime);
            await _deliveryRepo.AddAsync(delivery);
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

    public async Task<Result> StartDelivery(int deliveryId)
    {
        return await FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.StartDelivery();
        });
    }

    public async Task<Result> CompleteDelivery(int deliveryId)
    {
        return await FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.Complete();
        }, PublishStatusChangedEvent);
    }

    public async Task<Result> PostponeDelivery(int deliveryId)
    {
        return await FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.Postpone();
        }, PublishStatusChangedEvent);
    }

    public async Task<Result> CancelDelivery(int deliveryId)
    {
        return await FindAndUpdateStatus(deliveryId, (delivery) =>
        {
            delivery.Cancel();
        }, PublishStatusChangedEvent);
    }

    #region Helpers
    private async Task<Result> FindAndUpdateStatus(int deliveryId,
        Action<Delivery> statusChange, Action<Delivery>? afterUpdateDo = null)
    {
        var delivery = await _deliveryRepo
            .SingleOrDefaultAsync(new DeliveryByIdSpec(deliveryId));
        if (delivery == null)
        {
            return Result.NotFound();
        }

        try
        {
            statusChange(delivery);
            await _deliveryRepo.UpdateAsync(delivery);
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
