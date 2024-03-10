namespace Bazaar.MediaServer.Services;

public class OnDiskImageService(IWebHostEnvironment hostEnv, IConfiguration config)
{
    private const string _directory = "images";
    private readonly uint _httpsPort = uint.Parse(config["ExternalHttpsPort"]!);
    private readonly IWebHostEnvironment _hostEnv = hostEnv;

    public async Task<string> SaveImage(string originalFileName, Image image)
    {
        var extension = image.Metadata.DecodedImageFormat!.FileExtensions.First();
        var filename = $"{HashSha256(originalFileName)}.{extension}";
        var pathToDirectory = Path.Combine(_hostEnv.WebRootPath, _directory);
        var pathToFile = Path.Combine(pathToDirectory, filename);
        if (File.Exists(pathToFile))
        {
            File.Delete(pathToFile);
        }
        Directory.CreateDirectory(pathToDirectory);
        await image.SaveAsync(pathToFile);

        var url = Url.Combine($"https://localhost:{_httpsPort}", _directory, filename);
        return url;
    }

    private static string HashSha256(string input)
    {
        var builder = new StringBuilder();
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        foreach (var b in hashBytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
