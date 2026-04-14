using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Category;
using Lumiere.Tests.Integration.Setup;

namespace Lumiere.Tests.Integration.Controllers;

public class CategoriesControllerTests : IntegrationTestBase
{
    // public CategoriesControllerTests(TestWebAppFactory factory) : base(factory) { }

    // [Fact]
    // public async Task GetAll_WithoutAuth_Returns200()
    // {
    //     ClearAuthentication();
    //     var response = await Client.GetAsync("/api/categories?page=1&pageSize=10");
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task Create_AsAdmin_Returns201()
    // {
    //     await AuthenticateAsAdminAsync();

    //     var request = new CreateCategoryRequest { Name = "Bracelets", Slug = "bracelets", Description = ""};
    //     var response = await Client.PostAsJsonAsync("/api/categories", request);

    //     response.StatusCode.Should().Be(HttpStatusCode.Created);
    // }

    // [Fact]
    // public async Task Create_AsRegularUser_Returns403()
    // {
    //     await AuthenticateAsUserAsync();

    //     var request = new CreateCategoryRequest { Name = "Earrings" };
    //     var response = await Client.PostAsJsonAsync("/api/categories", request);

    //     response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    // }

    // [Fact]
    // public async Task Delete_NonExistentCategory_Returns404()
    // {
    //     await AuthenticateAsAdminAsync();

    //     var response = await Client.DeleteAsync("/api/categories/99999");

    //     response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    // }

    public CategoriesControllerTests(TestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_WithoutAuth_Returns200()
    {
        var response = await Client.GetAsync("/api/categories?page=1&pageSize=10");

        // Extract the error message from the API response
        var errorMessage = await response.Content.ReadAsStringAsync();

        // If it fails, the test runner will now print EXACTLY what the API complained about
        response.StatusCode.Should().Be(HttpStatusCode.OK, "API Error: {0}", errorMessage);
    }

    [Fact]
    public async Task Create_AsAdmin_Returns201()
    {
        await AuthenticateAsAdminAsync();

        var request = new CreateCategoryRequest { Name = "Bracelets", Slug = "bracelets", Description = "Bracelet Category" };
        var response = await Client.PostAsJsonAsync("/api/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AsRegularUser_Returns403()
    {
        await AuthenticateAsUserAsync();

        var request = new CreateCategoryRequest { Name = "Earrings" };
        var response = await Client.PostAsJsonAsync("/api/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_NonExistentCategory_Returns404()
    {
        await AuthenticateAsAdminAsync();

        var response = await Client.DeleteAsync("/api/categories/99999");

        // The BaseApiController correctly maps "not found" messages to 404 Not Found.
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
