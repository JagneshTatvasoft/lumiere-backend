using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.User;
using Lumiere.Tests.Integration.Setup;

namespace Lumiere.Tests.Integration.Controllers;

public class UsersControllerTests : IntegrationTestBase
{
    // public UsersControllerTests(TestWebAppFactory factory) : base(factory) { }

    // // ---------------------------------------------------------------
    // // GET /api/users  — Admin only
    // // ---------------------------------------------------------------

    // [Fact]
    // public async Task GetAll_AsAdmin_Returns200()
    // {
    //     await AuthenticateAsAdminAsync();

    //     var response = await Client.GetAsync("/api/users");

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task GetAll_AsRegularUser_Returns403()
    // {
    //     await AuthenticateAsUserAsync();

    //     var response = await Client.GetAsync("/api/users");

    //     response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    // }

    // [Fact]
    // public async Task GetAll_WithoutAuth_Returns401()
    // {
    //     ClearAuthentication();

    //     var response = await Client.GetAsync("/api/users");

    //     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    // }

    // // ---------------------------------------------------------------
    // // GET /api/users/{id}  — own profile only, unless Admin
    // // ---------------------------------------------------------------

    // [Fact]
    // public async Task GetById_UserFetchesOwnProfile_Returns200()
    // {
    //     // User Id=2 logs in and fetches their own profile
    //     await AuthenticateAsUserAsync();

    //     var response = await Client.GetAsync("/api/users/2");

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task GetById_UserFetchesAnotherUserProfile_Returns403()
    // {
    //     // User Id=2 tries to fetch User Id=1 — should be forbidden
    //     await AuthenticateAsUserAsync();

    //     var response = await Client.GetAsync("/api/users/1");

    //     response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    // }

    // [Fact]
    // public async Task GetById_AdminFetchesAnyProfile_Returns200()
    // {
    //     // Admin can fetch anyone's profile
    //     await AuthenticateAsAdminAsync();

    //     var response = await Client.GetAsync("/api/users/2");

    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // // ---------------------------------------------------------------
    // // PUT /api/users/{id}  — users cannot escalate their own role
    // // ---------------------------------------------------------------

    // [Fact]
    // public async Task Update_RegularUserCannotEscalateRole_RoleIsForced()
    // {
    //     await AuthenticateAsUserAsync();

    //     // Try to set RoleId = 2 (Admin) — controller must override this to 1
    //     var request = new UpdateUserRequest
    //     {
    //         Name = "Updated Name",
    //         Email = "user@test.com",
    //         Country = "IN",
    //         RoleId = 2   // trying to become Admin!
    //     };

    //     var response = await Client.PutAsJsonAsync("/api/users/2", request);

    //     // Request succeeds but role should NOT have changed
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);

    //     // Verify the role was NOT escalated
    //     var profile = await Client.GetAsync("/api/users/2");
    //     var body = await profile.Content.ReadFromJsonAsync<dynamic>();
    //     string role = body!.GetProperty("data").GetProperty("role").GetString()!;
    //     role.Should().Be("User"); // still User, not Admin
    // }

    public UsersControllerTests(TestWebAppFactory factory) : base(factory) { }

    // [Fact]
    // public async Task GetAll_AsAdmin_Returns200()
    // {
    //     await AuthenticateAsAdminAsync();
    //     var response = await Client.GetAsync("/api/users");
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    // }

    // [Fact]
    // public async Task GetAll_AsRegularUser_Returns403()
    // {
    //     await AuthenticateAsUserAsync();
    //     var response = await Client.GetAsync("/api/users");
    //     response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    // }

    // [Fact]
    // public async Task GetAll_WithoutAuth_Returns401()
    // {
    //     ClearAuthentication();
    //     var response = await Client.GetAsync("/api/users");
    //     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    // }

    
}
