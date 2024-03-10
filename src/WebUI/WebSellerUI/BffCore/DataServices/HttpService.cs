namespace WebSellerUI.DataServices;

public class HttpService
{
    protected readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content)
            ?? throw new Exception($"Error parsing JSON: Invalid JSON object, attempted to parse value: {content}");
    }

    protected static string ErrorStatusMessage(HttpStatusCode statusCode)
        => $"Request unsuccessful, status code: {statusCode}";
}
