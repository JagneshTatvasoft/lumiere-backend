using AutoMapper;
using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.DTOs.Request.Category;
using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.DTOs.Response.Article;
using Lumiere.Application.DTOs.Response.Category;
using Lumiere.Application.DTOs.Response.Favorite;
using Lumiere.Application.DTOs.Response.User;
using Lumiere.Domain;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserResponse>()
            .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role != null ? s.Role.Name : string.Empty));

        CreateMap<CreateUserRequest, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.FavoriteArticles, o => o.Ignore());

        CreateMap<UpdateUserRequest, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.FavoriteArticles, o => o.Ignore());

        // Article
        CreateMap<Article, ArticleResponse>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.LikesCount, o => o.MapFrom(s =>
                s.FavoriteArticles != null
                    ? s.FavoriteArticles.Count(f => f.ReactionType == ReactionType.Like)
                    : 0))
            .ForMember(d => d.DislikesCount, o => o.MapFrom(s =>
                s.FavoriteArticles != null
                    ? s.FavoriteArticles.Count(f => f.ReactionType == ReactionType.Dislike)
                    : 0));

        CreateMap<CreateArticleRequest, Article>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.FavoriteArticles, o => o.Ignore());

        CreateMap<UpdateArticleRequest, Article>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.FavoriteArticles, o => o.Ignore());

        // Category
        CreateMap<Category, CategoryResponse>()
            .ForMember(d => d.ArticleCount, o => o.MapFrom(s =>
                s.Articles != null ? s.Articles.Count(a => !a.IsDeleted) : 0));

        CreateMap<CreateCategoryRequest, Category>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Articles, o => o.Ignore());

        CreateMap<UpdateCategoryRequest, Category>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore())
            .ForMember(d => d.Articles, o => o.Ignore());

        // Favorite
        CreateMap<FavoriteArticle, FavoriteResponse>()
            .ForMember(d => d.ArticleName, o => o.MapFrom(s => s.Article != null ? s.Article.Name : string.Empty))
            .ForMember(d => d.ArticleImageUrl, o => o.MapFrom(s => s.Article != null ? s.Article.ImageUrl : null))
            .ForMember(d => d.ArticlePrice, o => o.MapFrom(s => s.Article != null ? s.Article.Price : 0));
    }
}


public class ApplicationAssemblyMarker {};