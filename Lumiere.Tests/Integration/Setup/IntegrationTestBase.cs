using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Lumiere.Application.DTOs.Request.Auth;

namespace Lumiere.Tests.Integration.Setup;

public abstract class IntegrationTestBase : IClassFixture<TestWebAppFactory>
{
    protected readonly HttpClient Client;
    private readonly TestWebAppFactory _factory;

    protected IntegrationTestBase(TestWebAppFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient(); // creates an HTTP client pointed at your in-memory API
    }

    // Call this to get an Admin JWT and set it on the client
    protected async Task AuthenticateAsAdminAsync()
    {
        var token = await GetTokenAsync("admin@test.com", "Admin@123");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // Call this to get a regular User JWT
    protected async Task AuthenticateAsUserAsync()
    {
        var token = await GetTokenAsync("user@test.com", "User@123");
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    // Remove auth header — simulates anonymous request
    protected void ClearAuthentication()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    private async Task<string> GetTokenAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Test login failed for {email}. " +
                $"Status: {response.StatusCode}. " +
                $"Response: {errorBody}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        // Navigates: { "data": { "token": "..." } }
        return doc.RootElement
                  .GetProperty("data")
                  .GetProperty("token")
                  .GetString()!;
    }
}
