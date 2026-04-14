using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Tests.Integration.Setup;

namespace Lumiere.Tests.Integration.Controllers;

public class ArticlesControllerTests : IntegrationTestBase
{
    public ArticlesControllerTests(TestWebAppFactory factory) : base(factory) { }

    // ---------------------------------------------------------------
    // GET /api/articles  — public endpoint, no auth needed
    // ---------------------------------------------------------------

    // [Fact]
    // public async Task GetAll_WithoutAuth_Returns200()
    // {
    //     // Arrange — no login needed, AllowAnonymous
    //     ClearAuthentication();

    //     var response = await Client.GetAsync("/api/articles");

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task GetAll_WithPagination_ReturnsPagedResult()
    // {
    //     ClearAuthentication();

    //     var response = await Client.GetAsync("/api/articles?page=1&pageSize=10");
    //     var body = await response.Content.ReadFromJsonAsync<dynamic>();

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //     // PagedResult shape: { data: { items: [], totalCount: N, page: 1, pageSize: 10 } }
    //     body!.GetProperty("data").GetProperty("items").GetArrayLength()
    //          .Should().BeGreaterThanOrEqualTo(0);
    // }

    // // ---------------------------------------------------------------
    // // GET /api/articles/{id}
    // // ---------------------------------------------------------------

    // [Fact]
    // public async Task GetById_WithExistingId_Returns200()
    // {
    //     ClearAuthentication();

    //     // Article with Id=1 is seeded in TestWebAppFactory
    //     var response = await Client.GetAsync("/api/articles/1");

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task GetById_WithNonExistentId_Returns404()
    // {
    //     ClearAuthentication();

    //     var response = await Client.GetAsync("/api/articles/99999");

    //     response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    // }

    // // ---------------------------------------------------------------
    // // POST /api/articles  — Admin only
    // // ---------------------------------------------------------------

    // [Fact]
    // public async Task Create_AsAdmin_Returns201()
    // {
    //     // Arrange — must be admin
    //     await AuthenticateAsAdminAsync();

    //     var request = new CreateArticleRequest
    //     {
    //         Name = "Diamond Bracelet",
    //         Description = "",
    //         ShortDescription = "Stunning diamond bracelet",
    //         Price = 999.99m,
    //         Stock = 5,
    //         CategoryId = 1   // seeded in factory
    //     };

    //     var response = await Client.PostAsJsonAsync("/api/articles", request);

    //     response.StatusCode.Should().Be(HttpStatusCode.Created);
    // }

    // [Fact]
    // public async Task Create_AsRegularUser_Returns403()
    // {
    //     // Regular users cannot create articles — should get Forbidden
    //     await AuthenticateAsUserAsync();

    //     var request = new CreateArticleRequest
    //     {
    //         Name = "Unauthorized Article",
    //         Price = 100m,
    //         CategoryId = 1
    //     };

    //     var response = await Client.PostAsJsonAsync("/api/articles", request);

    //     response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    // }

    // [Fact]
    // public async Task Create_WithoutAuth_Returns401()
    // {
    //     // No token at all — should get Unauthorized
    //     ClearAuthentication();

    //     var request = new CreateArticleRequest { Name = "No Auth Article", Price = 100m, CategoryId = 1 };

    //     var response = await Client.PostAsJsonAsync("/api/articles", request);

    //     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    // }

    // [Fact]
    // public async Task Create_WithInvalidData_Returns400()
    // {
    //     await AuthenticateAsAdminAsync();

    //     // Missing required fields — FluentValidation should catch this
    //     var request = new CreateArticleRequest { Name = "" };

    //     var response = await Client.PostAsJsonAsync("/api/articles", request);

    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    // }

    // // ---------------------------------------------------------------
    // // DELETE /api/articles/{id}  — Admin only
    // // ---------------------------------------------------------------

    // [Fact]
    // public async Task Delete_AsAdmin_Returns200()
    // {
    //     await AuthenticateAsAdminAsync();

    //     // Article Id=1 is seeded
    //     var response = await Client.DeleteAsync("/api/articles/1");

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task Delete_AsRegularUser_Returns403()
    // {
    //     await AuthenticateAsUserAsync();

    //     var response = await Client.DeleteAsync("/api/articles/1");

    //     response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    // }

    [Fact]
    public async Task GetAll_WithoutAuth_Returns200()
    {
        ClearAuthentication();
        var response = await Client.GetAsync("/api/articles");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsPagedResult()
    {
        ClearAuthentication();

        var response = await Client.GetAsync("/api/articles?page=1&pageSize=10");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Fix: Use JsonDocument to extract int safely to avoid RuntimeBinderException
        var itemsCount = doc.RootElement.GetProperty("data").GetProperty("items").GetArrayLength();
        itemsCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetById_WithExistingId_Returns200()
    {
        ClearAuthentication();
        var response = await Client.GetAsync("/api/articles/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        ClearAuthentication();
        var response = await Client.GetAsync("/api/articles/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_AsAdmin_Returns201()
    {
        await AuthenticateAsAdminAsync();

        var request = new CreateArticleRequest
        {
            Name = "Diamond Bracelet",
            Description = "A high-quality diamond bracelet.", // Fix: Replaced empty string to bypass 400 validation
            ShortDescription = "Stunning diamond bracelet",
            Price = 999.99m,
            Stock = 5,
            CategoryId = 1
        };

        var response = await Client.PostAsJsonAsync("/api/articles", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_AsRegularUser_Returns403()
    {
        await AuthenticateAsUserAsync();

        var request = new CreateArticleRequest
        {
            Name = "Unauthorized Article",
            Price = 100m,
            CategoryId = 1
        };

        var response = await Client.PostAsJsonAsync("/api/articles", request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithoutAuth_Returns401()
    {
        ClearAuthentication();
        var request = new CreateArticleRequest { Name = "No Auth Article", Price = 100m, CategoryId = 1 };
        var response = await Client.PostAsJsonAsync("/api/articles", request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithInvalidData_Returns400()
    {
        await AuthenticateAsAdminAsync();
        var request = new CreateArticleRequest { Name = "" };
        var response = await Client.PostAsJsonAsync("/api/articles", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_AsAdmin_Returns200()
    {
        await AuthenticateAsAdminAsync();
        var response = await Client.DeleteAsync("/api/articles/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_AsRegularUser_Returns403()
    {
        await AuthenticateAsUserAsync();
        var response = await Client.DeleteAsync("/api/articles/1");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
