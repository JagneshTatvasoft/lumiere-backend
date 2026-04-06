using AutoMapper;
using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Article;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
 
    public ArticleService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
 
    public async Task<ApiResponse<PagedResult<ArticleResponse>>> GetAllAsync(ArticleQueryParams q, CancellationToken ct = default)
    {
        var paged = await _uow.Articles.GetFilteredAsync(q, ct);
        var mapped = new PagedResult<ArticleResponse>
        {
            Items = _mapper.Map<List<ArticleResponse>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
        return ApiResponse<PagedResult<ArticleResponse>>.Ok(mapped);
    }
 
    public async Task<ApiResponse<ArticleResponse>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var article = await _uow.Articles.GetWithCategoryAsync(id, ct);
        if (article == null || article.IsDeleted)
            return ApiResponse<ArticleResponse>.Fail("Article not found.");
 
        return ApiResponse<ArticleResponse>.Ok(_mapper.Map<ArticleResponse>(article));
    }
 
    public async Task<ApiResponse<ArticleResponse>> CreateAsync(CreateArticleRequest request, CancellationToken ct = default)
    {
        var categoryExists = await _uow.Categories.ExistsAsync(c => c.Id == request.CategoryId && !c.IsDeleted, ct);
        if (!categoryExists)
            return ApiResponse<ArticleResponse>.Fail("Category not found.");
 
        var article = _mapper.Map<Article>(request);
        await _uow.Articles.AddAsync(article, ct);
        await _uow.SaveChangesAsync(ct);
 
        var saved = await _uow.Articles.GetWithCategoryAsync(article.Id, ct);
        return ApiResponse<ArticleResponse>.Ok(_mapper.Map<ArticleResponse>(saved), "Article created.");
    }
 
    public async Task<ApiResponse<ArticleResponse>> UpdateAsync(int id, UpdateArticleRequest request, CancellationToken ct = default)
    {
        var article = await _uow.Articles.GetByIdAsync(id, ct);
        if (article == null || article.IsDeleted)
            return ApiResponse<ArticleResponse>.Fail("Article not found.");
 
        var categoryExists = await _uow.Categories.ExistsAsync(c => c.Id == request.CategoryId && !c.IsDeleted, ct);
        if (!categoryExists)
            return ApiResponse<ArticleResponse>.Fail("Category not found.");
 
        _mapper.Map(request, article);
        article.UpdatedAt = DateTime.UtcNow;
 
        await _uow.Articles.UpdateAsync(article, ct);
        await _uow.SaveChangesAsync(ct);
 
        var updated = await _uow.Articles.GetWithCategoryAsync(id, ct);
        return ApiResponse<ArticleResponse>.Ok(_mapper.Map<ArticleResponse>(updated), "Article updated.");
    }
 
    public async Task<ApiResponse> DeleteAsync(int id, CancellationToken ct = default)
    {
        var article = await _uow.Articles.GetByIdAsync(id, ct);
        if (article == null || article.IsDeleted)
            return ApiResponse.Fail("Article not found.");
 
        article.IsDeleted = true;
        article.DeletedAt = DateTime.UtcNow;
        await _uow.Articles.UpdateAsync(article, ct);
        await _uow.SaveChangesAsync(ct);
 
        return ApiResponse.Ok("Article deleted.");
    }
}