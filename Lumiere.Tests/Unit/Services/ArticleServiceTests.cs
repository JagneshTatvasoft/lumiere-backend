using AutoMapper;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Services;
using Lumiere.Domain.Entities;
using Lumiere.Tests.Unit.Helpers;
using Moq;

namespace Lumiere.Tests.Unit.Services;

public class ArticleServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IArticleRepository> _articleRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly IMapper _mapper;
    private readonly ArticleService _sut;


    public ArticleServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _articleRepoMock = new Mock<IArticleRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _mapper = MapperFactory.Create();

        // Wire UoW mock to return our repo mocks
        _uowMock.Setup(u => u.Articles).Returns(_articleRepoMock.Object);
        _uowMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);

        _sut = new ArticleService(_uowMock.Object, _mapper);
    }

    // GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_WhenArticleExists_ReturnsSuccess()
    {
        // Arrange
        var article = new Article
        {
            Id = 1, Name = "Test Article", IsDeleted = false,
            Category = new Category { Id = 1, Name = "Rings" }
        };
        _articleRepoMock
            .Setup(r => r.GetWithCategoryAsync(1, default))
            .ReturnsAsync(article);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Test Article");
    }


    [Fact]
    public async Task GetByIdAsync_WhenArticleNotFound_ReturnsFail()
    {
        _articleRepoMock
            .Setup(r => r.GetWithCategoryAsync(99, default))
            .ReturnsAsync((Article?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

     [Fact]
    public async Task GetByIdAsync_WhenArticleIsDeleted_ReturnsFail()
    {
        // article exists in DB but is soft-deleted
        var article = new Article { Id = 1, IsDeleted = true };
        _articleRepoMock
            .Setup(r => r.GetWithCategoryAsync(1, default))
            .ReturnsAsync(article);

        var result = await _sut.GetByIdAsync(1);

        result.Success.Should().BeFalse();
    }

    // CreateAsync
        [Fact]
    public async Task CreateAsync_WithValidCategory_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateArticleRequest
        {
            Name = "Pearl Necklace", CategoryId = 1, Price = 299.99m
        };
        var savedArticle = new Article
        {
            Id = 10, Name = "Pearl Necklace",
            Category = new Category { Id = 1, Name = "Necklaces" }
        };

        _categoryRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), default))
            .ReturnsAsync(true); 
        _articleRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Article>(), default))
            .ReturnsAsync((Article a, CancellationToken _) => a);

        _uowMock
            .Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        _articleRepoMock
            .Setup(r => r.GetWithCategoryAsync(It.IsAny<int>(), default))
            .ReturnsAsync(savedArticle);

        var result = await _sut.CreateAsync(request);

        result.Success.Should().BeTrue();
        result.Message.Should().Contain("created");
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once); 
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCategory_ReturnsFail()
    {
        var request = new CreateArticleRequest { CategoryId = 999 };

        _categoryRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), default))
            .ReturnsAsync(false); 

        var result = await _sut.CreateAsync(request);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Category not found");
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never); 
    }

    // DeleteAsync
        [Fact]
    public async Task DeleteAsync_WhenArticleExists_SoftDeletesIt()
    {
        // Arrange
        var article = new Article { Id = 1, IsDeleted = false };
        _articleRepoMock
            .Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(article);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteAsync(1);

        // Assert
        result.Success.Should().BeTrue();
        article.IsDeleted.Should().BeTrue();          
        article.DeletedAt.Should().NotBeNull();        
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAlreadyDeleted_ReturnsFail()
    {
        var article = new Article { Id = 1, IsDeleted = true };
        _articleRepoMock
            .Setup(r => r.GetByIdAsync(1, default))
            .ReturnsAsync(article);

        var result = await _sut.DeleteAsync(1);

        result.Success.Should().BeFalse();
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
