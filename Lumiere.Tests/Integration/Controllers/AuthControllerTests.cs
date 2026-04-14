using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Auth;
using Lumiere.Tests.Integration.Setup;

namespace Lumiere.Tests.Integration.Controllers;

public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(TestWebAppFactory factory) : base(factory) { }

    // ---------------------------------------------------------------
    // POST /api/auth/login
    // ---------------------------------------------------------------

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        // Arrange
        var request = new LoginRequest { Email = "admin@test.com", Password = "Admin@123" };

        // Act — real HTTP POST to your running API
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        // Token must be present and not empty
        string token = body!.GetProperty("data").GetProperty("token").GetString()!;
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns400()
    {
        var request = new LoginRequest { Email = "admin@test.com", Password = "WrongPassword" };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns400()
    {
        var request = new LoginRequest { Email = "ghost@test.com", Password = "any" };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithEmptyBody_Returns400WithValidationErrors()
    {
        // Arrange — FluentValidation should catch this before hitting the service
        var request = new LoginRequest { Email = "", Password = "" };

        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---------------------------------------------------------------
    // POST /api/auth/register
    // ---------------------------------------------------------------

    [Fact]
    public async Task Register_WithNewEmail_Returns200WithToken()
    {
        var request = new RegisterRequest
        {
            Name = "New User",
            Email = "brandnew@test.com",
            Password = "NewPass@123",
            Country = "IN"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithExistingEmail_Returns400()
    {
        // admin@test.com is already seeded in TestWebAppFactory
        var request = new RegisterRequest
        {
            Name = "Duplicate",
            Email = "admin@test.com",
            Password = "Pass@123",
            Country = "IN"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
