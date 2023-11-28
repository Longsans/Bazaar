namespace Bazaar.ScheduledTasks.BackgroundJobs;

// Request disposal for Lots unfulfillable beyond policy duration
public class RequestDisposalForLotsUbpdJob : IBackgroundJob
{
    private readonly HttpClient _httpClient;
    private readonly string _fbbInventoryUri;
    private readonly ILogger<RequestDisposalForLotsUbpdJob> _logger;

    public RequestDisposalForLotsUbpdJob(
        HttpClient httpClient, IConfiguration config,
        ILogger<RequestDisposalForLotsUbpdJob> logger)
    {
        _httpClient = httpClient;
        _fbbInventoryUri = config["FbbInventoryApi"]!;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var requestBody = new LotsUbpdDisposalRequest(true);
        var patchContent = new StringContent(JsonSerializer.Serialize(requestBody),
            System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(
            RequestDisposalForLotsUbpdEndpoint, patchContent);

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

    private string RequestDisposalForLotsUbpdEndpoint
        => $"{_fbbInventoryUri}/api/lots/unf-disposal-requests";
}
