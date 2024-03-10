using SixLabors.ImageSharp.Formats.Png;
namespace Bazaar.Catalog.Application.Services;

public class EventBasedImageService : IImageService
{
    public string ImageHostLocation => Url.Combine(_imageHostUrl, _directory);
    private const string _directory = "images";
    private readonly string _imageHostUrl;
    private readonly IEventBus _eventBus;

    public EventBasedImageService(IConfiguration config, IEventBus eventBus)
    {
        _imageHostUrl = config["MediaServerUrl"]!;
        _eventBus = eventBus;
    }

    public async Task<string?> SaveImageForProduct(string productId, Image image)
    {
        var dataUri = image.ToBase64String(image.Metadata.DecodedImageFormat ?? PngFormat.Instance);
        var contentStart = dataUri.IndexOf(',') + 1;
        if (contentStart == dataUri.Length)
        {
            throw new ArgumentException("Image content is empty.");
        }
        var base64String = dataUri[contentStart..];
        _eventBus.Publish(new ProductImageUpdatedIntegrationEvent(productId, base64String));
        return await Task.FromResult(null as string);
    }
}
