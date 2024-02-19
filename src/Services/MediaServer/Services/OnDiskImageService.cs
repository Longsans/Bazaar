using Flurl;

namespace Bazaar.MediaServer.Services;

public class OnDiskImageService(IWebHostEnvironment hostEnv, IConfiguration config)
{
    private const string _directory = "images";
    private readonly uint _httpsPort = uint.Parse(config["ExternalHttpsPort"]!);
    private readonly IWebHostEnvironment _hostEnv = hostEnv;

    public async Task<string> SaveImageForProduct(string productId, Image image)
    {
        var extension = image.Metadata.DecodedImageFormat!.FileExtensions.First();
        var filename = $"{productId.ToLower().Replace('-', '_')}.{extension}";
        var dirPath = Path.Combine(_hostEnv.WebRootPath, _directory);
        var filePath = Path.Combine(dirPath, filename);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        Directory.CreateDirectory(dirPath);
        await image.SaveAsync(filePath);

        var url = Url.Combine($"https://localhost:{_httpsPort}", _directory, filename);
        return url;
    }
}
