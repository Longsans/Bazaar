namespace Bazaar.MediaServer.EventHandling;

public class ProductImageUpdatedIntegrationEventHandler(OnDiskImageService imageService, IEventBus eventBus)
    : IIntegrationEventHandler<ProductImageUpdatedIntegrationEvent>
{
    private readonly OnDiskImageService _imageService = imageService;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(ProductImageUpdatedIntegrationEvent @event)
    {
        var imageBytes = Convert.FromBase64String(@event.Base64EncodedImage);
        var imageUrl = await _imageService.SaveImageForProduct(@event.ProductId, Image.Load(imageBytes));
        _eventBus.Publish(new ProductImageSavedIntegrationEvent(@event.ProductId, imageUrl));
    }
}
