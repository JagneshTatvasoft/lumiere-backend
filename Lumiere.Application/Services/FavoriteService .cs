using AutoMapper;
using Lumiere.Application.DTOs.Request.Favorite;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Favorite;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Domain;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
 
    public FavoriteService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
 
    public async Task<ApiResponse<ToggleFavoriteResponse>> ToggleAsync(ToggleFavoriteRequest request, CancellationToken ct = default)
    {
        var existing = await _uow.Favorites.GetByUserAndArticleAsync(request.UserId, request.ArticleId, ct);
 
        // If same reaction → remove it
        if (existing != null && existing.ReactionType == request.ReactionType)
        {
            await _uow.Favorites.DeleteAsync(existing, ct);
            await _uow.SaveChangesAsync(ct);
            return ApiResponse<ToggleFavoriteResponse>.Ok(new ToggleFavoriteResponse
            {
                WasRemoved = true,
                IsLiked = false,
                IsDisliked = false
            });
        }
 
        // If different reaction → update
        if (existing != null)
        {
            existing.ReactionType = request.ReactionType;
            existing.UpdatedAt = DateTime.UtcNow;
            await _uow.Favorites.UpdateAsync(existing, ct);
            await _uow.SaveChangesAsync(ct);
        }
        else
        {
            // New reaction
            var favorite = new FavoriteArticle
            {
                UserId = request.UserId,
                ArticleId = request.ArticleId,
                ReactionType = request.ReactionType,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Favorites.AddAsync(favorite, ct);
            await _uow.SaveChangesAsync(ct);
            existing = favorite;
        }
 
        return ApiResponse<ToggleFavoriteResponse>.Ok(new ToggleFavoriteResponse
        {
            WasRemoved = false,
            IsLiked = existing.ReactionType == ReactionType.Like,
            IsDisliked = existing.ReactionType == ReactionType.Dislike,
            CurrentReaction = existing.ReactionType
        });
    }
 
    public async Task<ApiResponse<List<FavoriteResponse>>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var favorites = await _uow.Favorites.GetByUserIdAsync(userId, ct);
        return ApiResponse<List<FavoriteResponse>>.Ok(_mapper.Map<List<FavoriteResponse>>(favorites));
    }
}