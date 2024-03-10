namespace Bazaar.MediaServer.EventHandling;

public class ProductImageUpdatedIntegrationEventHandler(OnDiskImageService imageService, IEventBus eventBus)
    : IIntegrationEventHandler<ProductImageUpdatedIntegrationEvent>
{
    private readonly OnDiskImageService _imageService = imageService;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(ProductImageUpdatedIntegrationEvent @event)
    {
        try
        {
            var bytes = Convert.FromBase64String(@event.Base64ImageString);
            var image = Image.Load(bytes);
            // We use the content of the image appended with its product ID to make the filename
            var imageUrl = await _imageService.SaveImage(@event.Base64ImageString + @event.ProductId, image);
            _eventBus.Publish(new ProductImageSavedIntegrationEvent(@event.ProductId, imageUrl));
        }
        catch (Exception ex)
        {
            _eventBus.Publish(new ProductImageFailedToSaveIntegrationEvent(@event.ProductId, ex.Message));
        }
    }
}
