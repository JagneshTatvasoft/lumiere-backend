using FluentValidation;
using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumiere.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequest> _createValidator;
    private readonly IValidator<UpdateUserRequest> _updateValidator;

    public UsersController(
        IUserService userService,
        IValidator<CreateUserRequest> createValidator,
        IValidator<UpdateUserRequest> updateValidator)
    {
        _userService = userService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Get all users (Admin only) with pagination and filtering.</summary>
    [HttpGet]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryParams queryParams, CancellationToken ct)
    {
        return StatusCode(503, new { message = "Feature temporarily unavailable" });
        // var result = await _userService.GetAllAsync(queryParams, ct);
        // return HandleResponse(result);
    }


}
