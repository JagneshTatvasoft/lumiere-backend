using FluentValidation;
using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumiere.API.Controllers;

public class ArticlesController : BaseApiController
{
     private readonly IArticleService _articleService;
    private readonly IValidator<CreateArticleRequest> _createValidator;
    private readonly IValidator<UpdateArticleRequest> _updateValidator;
 
    public ArticlesController(
        IArticleService articleService,
        IValidator<CreateArticleRequest> createValidator,
        IValidator<UpdateArticleRequest> updateValidator)
    {
        _articleService = articleService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
 
    /// <summary> Get all articles with optional filtering and pagination. </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ArticleQueryParams queryParams, CancellationToken ct)
    {
        var result = await _articleService.GetAllAsync(queryParams, ct);
        return HandleResponse(result);
    }
 
    /// <summary> Get a single article by ID. </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _articleService.GetByIdAsync(id, ct);
        return HandleResponse(result);
    }
 
    /// <summary> Create an article (Admin only).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateArticleRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });
 
        var result = await _articleService.CreateAsync(request, ct);
        return HandleResponse(result, result.Data?.Id ?? 0);
    }
 
    /// <summary>Update an article (Admin only).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArticleRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });
 
        var result = await _articleService.UpdateAsync(id, request, ct);
        return HandleResponse(result);
    }
 
    /// <summary> Soft-delete an article (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _articleService.DeleteAsync(id, ct);
        return HandleResponse(result);
    }
}
