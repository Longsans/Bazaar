namespace Bazaar.Catalog.Application.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Saves the image in image host using <paramref name="productId"/> (lowercased and <b>-</b> replaced with <b>_</b> ) as file name
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="image"></param>
    /// <returns>The name of the saved image, e.g. <i>prod_1.png</i></returns>
    Task<string?> SaveImageForProduct(string productId, Image image);

    string ImageHostLocation { get; }
}
