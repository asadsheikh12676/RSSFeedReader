using RSSFeedReader.UI.Models;
using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

public class SubscriptionApiClient
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "http://localhost:5151";

    public SubscriptionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Subscription> AddSubscriptionAsync(string url)
    {
        var request = new { url };
        var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/subscriptions", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Subscription>() ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<List<Subscription>> GetSubscriptionsAsync()
    {
        var response = await _httpClient.GetAsync($"{ApiBaseUrl}/api/subscriptions");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Subscription>>() ?? new List<Subscription>();
    }
}
