namespace Bazaar.ScheduledTasks.BackgroundJobs;

public class MarkOverdueUnfulfillableFbbInventoriesJob : IBackgroundJob
{
    private readonly HttpClient _httpClient;
    private readonly string _fbbInventoryUri;
    private readonly ILogger<MarkOverdueUnfulfillableFbbInventoriesJob> _logger;

    public MarkOverdueUnfulfillableFbbInventoriesJob(HttpClient httpClient, IConfiguration config,
        ILogger<MarkOverdueUnfulfillableFbbInventoriesJob> logger)
    {
        _httpClient = httpClient;
        _fbbInventoryUri = config["FbbInventoryApi"]!;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var requestBody = new MarkInventoriesToBeDisposedRequest(true);
        var patchContent = new StringContent(JsonSerializer.Serialize(requestBody),
            System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(
            OverdueUnfulfillableProductInventoriesEndpoint, patchContent);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "Successful daily scheduled task: Marked overdue unfulfillable FBB inventories to be disposed.");
        }
        else
        {
            _logger.LogError("Errors processing daily scheduled task: " +
                "Unable to mark overdue unfulfillable inventories for disposal.");
        }
    }

    private string OverdueUnfulfillableProductInventoriesEndpoint
        => $"{_fbbInventoryUri}/api/product-inventories?overdueUnfulfillable=true";
}
