namespace Bazaar.Disposal.Domain.Services;

public class DisposalOrderConclusionService(
    IDisposalOrderRepository disposalOrderRepo, IEventBus eventBus)
{
    private readonly IDisposalOrderRepository _disposalOrderRepo = disposalOrderRepo;
    private readonly IEventBus _eventBus = eventBus;

    public Result CompleteDisposal(int disposalOrderId)
    {
        var disposalOrder = _disposalOrderRepo.GetById(disposalOrderId);
        if (disposalOrder == null)
        {
            return Result.NotFound("Disposal order not found.");
        }

        try
        {
            disposalOrder.Complete();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _disposalOrderRepo.Update(disposalOrder);
        _eventBus.Publish(new DisposalOrderCompletedIntegrationEvent(
            disposalOrder.Id,
            disposalOrder.DisposeQuantities.Select(x =>
                new DisposedQuantity(x.LotNumber, x.UnitsToDispose)),
            DateTime.Now));
        return Result.Success();
    }

    public Result CancelDisposal(int disposalOrderId, string reason)
    {
        var disposalOrder = _disposalOrderRepo.GetById(disposalOrderId);
        if (disposalOrder == null)
        {
            return Result.NotFound("Disposal order not found.");
        }

        try
        {
            disposalOrder.Cancel(reason);
        }
        catch (ArgumentNullException ex)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = ex.ParamName,
                ErrorMessage = ex.Message,
            });
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _disposalOrderRepo.Update(disposalOrder);
        _eventBus.Publish(new DisposalOrderCancelledIntegrationEvent(
            disposalOrder.Id,
            disposalOrder.DisposeQuantities.Select(x =>
                new UndisposedQuantity(x.LotNumber, x.UnitsToDispose)),
            DateTime.Now));
        return Result.Success();
    }
}
