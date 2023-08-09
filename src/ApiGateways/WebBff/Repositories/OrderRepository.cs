namespace Bazaar.ApiGateways.WebBff.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string ORDERS_URI;
    private readonly HttpClient _httpClient;

    public OrderRepository(string orderUri, HttpClient httpClient)
    {
        ORDERS_URI = orderUri;
        _httpClient = httpClient;
    }

    public async Task<Order> Create(Order order)
    {
        var createCommand = new OrderCreateCommand(order);
        var requestContent = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(createCommand), System.Text.Encoding.UTF8, "application/json");

        var postResult = await _httpClient.PostAsync(ORDERS_URI, requestContent);
        postResult.EnsureSuccessStatusCode();

        var resultContent = await postResult.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Order>(resultContent);
    }
}
