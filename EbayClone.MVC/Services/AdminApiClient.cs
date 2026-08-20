using System.Net.Http.Headers;
using System.Net.Http.Json;
using EbayClone.MVC.Models;

namespace EbayClone.MVC.Services;

public class AdminApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
{
    public Task<LoginResponseModel?> LoginAsync(LoginInputModel input, CancellationToken cancellationToken) =>
        SendAsync<LoginResponseModel>(HttpMethod.Post, "api/auth/login", input, false, cancellationToken);

    public Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken) =>
        SendAsync<T>(HttpMethod.Get, path, null, true, cancellationToken);

    public Task<T?> PutAsync<T>(string path, object? body, CancellationToken cancellationToken) =>
        SendAsync<T>(HttpMethod.Put, path, body, true, cancellationToken);

    private async Task<T?> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        bool authorize,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        if (authorize)
        {
            var token = httpContextAccessor.HttpContext?.Session.GetString("AdminToken");
            if (string.IsNullOrWhiteSpace(token))
                throw new AdminApiException(401, "Phiên đăng nhập đã hết hạn.");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (body is not null)
            request.Content = JsonContent.Create(body);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new AdminApiException((int)response.StatusCode, message);
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }
}

public class AdminApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
