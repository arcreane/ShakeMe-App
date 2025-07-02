using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ShakeMe.Core.Http;
using Microsoft.Maui.Storage;

namespace ShakeMe.Core.Http;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:3000")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await SecureStorage.GetAsync("auth_token");
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        await AddAuthorizationHeaderAsync();

        var response = await _httpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode) return default;

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        await AddAuthorizationHeaderAsync();

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);

        if (!response.IsSuccessStatusCode) return default;

        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(raw, _jsonOptions);
    }
    
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        await AddAuthorizationHeaderAsync();

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);

        if (!response.IsSuccessStatusCode) return default;

        var raw = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(raw, _jsonOptions);
    }

}