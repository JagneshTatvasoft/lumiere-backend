using AutoMapper;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Favorite;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Services;
using Lumiere.Domain;
using Lumiere.Domain.Entities;
using Lumiere.Tests.Unit.Helpers;
using Moq;

namespace Lumiere.Tests.Unit.Services;

public class FavoriteServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IFavoriteRepository> _favRepoMock;
    private readonly IMapper _mapper;
    private readonly FavoriteService _sut;

    public FavoriteServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _favRepoMock = new Mock<IFavoriteRepository>();
        _mapper = MapperFactory.Create();
        _uowMock.Setup(u => u.Favorites).Returns(_favRepoMock.Object);
        _sut = new FavoriteService(_uowMock.Object, _mapper);
    }

    [Fact]
    public async Task ToggleAsync_WhenSameReactionExists_RemovesIt()
    {
        // Arrange — user already liked, sends like again → should remove
        var existing = new FavoriteArticle { UserId = 1, ArticleId = 5, ReactionType = ReactionType.Like };
        _favRepoMock.Setup(r => r.GetByUserAndArticleAsync(1, 5, default)).ReturnsAsync(existing);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var request = new ToggleFavoriteRequest { UserId = 1, ArticleId = 5, ReactionType = ReactionType.Like };

        var result = await _sut.ToggleAsync(request);

        result.Success.Should().BeTrue();
        result.Data!.WasRemoved.Should().BeTrue();
        result.Data.IsLiked.Should().BeFalse();
        _favRepoMock.Verify(r => r.DeleteAsync(existing, default), Times.Once);
    }

    [Fact]
    public async Task ToggleAsync_WhenDifferentReactionExists_UpdatesIt()
    {
        // Arrange — user previously liked, now sends dislike → should update
        var existing = new FavoriteArticle { UserId = 1, ArticleId = 5, ReactionType = ReactionType.Like };
        _favRepoMock.Setup(r => r.GetByUserAndArticleAsync(1, 5, default)).ReturnsAsync(existing);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var request = new ToggleFavoriteRequest { UserId = 1, ArticleId = 5, ReactionType = ReactionType.Dislike };

        var result = await _sut.ToggleAsync(request);

        result.Success.Should().BeTrue();
        result.Data!.WasRemoved.Should().BeFalse();
        result.Data.IsDisliked.Should().BeTrue();
        _favRepoMock.Verify(r => r.UpdateAsync(existing, default), Times.Once);
    }

    [Fact]
    public async Task ToggleAsync_WhenNoExistingReaction_CreatesNew()
    {
        // Arrange — user has never reacted
        _favRepoMock.Setup(r => r.GetByUserAndArticleAsync(1, 5, default)).ReturnsAsync((FavoriteArticle?)null);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var request = new ToggleFavoriteRequest { UserId = 1, ArticleId = 5, ReactionType = ReactionType.Like };

        var result = await _sut.ToggleAsync(request);

        result.Success.Should().BeTrue();
        result.Data!.IsLiked.Should().BeTrue();
        _favRepoMock.Verify(r => r.AddAsync(It.IsAny<FavoriteArticle>(), default), Times.Once);
    }
}
