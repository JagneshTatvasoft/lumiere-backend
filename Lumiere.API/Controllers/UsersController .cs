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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryParams queryParams, CancellationToken ct)
    {
        var result = await _userService.GetAllAsync(queryParams, ct);
        return HandleResponse(result);
    }
 
    /// <summary>Get a user by ID. Users can only fetch their own profile.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        // Non-admins can only see their own profile
        if (CurrentUserRole != "Admin" && CurrentUserId != id)
            return Forbid();
 
        var result = await _userService.GetByIdAsync(id, ct);
        return HandleResponse(result);
    }
 
    /// <summary>Create a new user (Admin only).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });
 
        var result = await _userService.CreateAsync(request, ct);
        return HandleResponse(result, result.Data?.Id ?? 0);
    }
 
    /// <summary>Update a user. Admins can update anyone; users can update themselves (no role change).</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        if (CurrentUserRole != "Admin" && CurrentUserId != id)
            return Forbid();
 
        // Non-admins cannot escalate their own role
        if (CurrentUserRole != "Admin")
            request.RoleId = 1;
 
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });
 
        var result = await _userService.UpdateAsync(id, request, ct);
        return HandleResponse(result);
    }
 
    /// <summary>Soft-delete a user (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _userService.DeleteAsync(id, ct);
        return HandleResponse(result);
    }
}
