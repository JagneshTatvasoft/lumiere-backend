using AutoMapper;
using Lumiere.Application.DTOs.Request.Category;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Category;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
 
    public CategoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
 
    public async Task<ApiResponse<List<CategoryResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _uow.Categories.GetAllAsync(ct);
        var active = categories.Where(c => !c.IsDeleted).ToList();
        return ApiResponse<List<CategoryResponse>>.Ok(_mapper.Map<List<CategoryResponse>>(active));
    }
 
    public async Task<ApiResponse<CategoryResponse>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetWithArticlesAsync(id, ct);
        if (category == null || category.IsDeleted)
            return ApiResponse<CategoryResponse>.Fail("Category not found.");
 
        return ApiResponse<CategoryResponse>.Ok(_mapper.Map<CategoryResponse>(category));
    }
 
    public async Task<ApiResponse<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var exists = await _uow.Categories.ExistsAsync(c => c.Name == request.Name && !c.IsDeleted, ct);
        if (exists)
            return ApiResponse<CategoryResponse>.Fail("Category with this name already exists.");
 
        var category = _mapper.Map<Category>(request);
        if (string.IsNullOrWhiteSpace(category.Slug))
            category.Slug = request.Name.ToLower().Replace(" ", "-");
 
        await _uow.Categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
 
        return ApiResponse<CategoryResponse>.Ok(_mapper.Map<CategoryResponse>(category), "Category created.");
    }
 
    public async Task<ApiResponse<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct);
        if (category == null || category.IsDeleted)
            return ApiResponse<CategoryResponse>.Fail("Category not found.");
 
        _mapper.Map(request, category);
        category.UpdatedAt = DateTime.UtcNow;
 
        await _uow.Categories.UpdateAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
 
        return ApiResponse<CategoryResponse>.Ok(_mapper.Map<CategoryResponse>(category), "Category updated.");
    }
 
    public async Task<ApiResponse> DeleteAsync(int id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct);
        if (category == null || category.IsDeleted)
            return ApiResponse.Fail("Category not found.");
 
        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        await _uow.Categories.UpdateAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
 
        return ApiResponse.Ok("Category deleted.");
    }
}