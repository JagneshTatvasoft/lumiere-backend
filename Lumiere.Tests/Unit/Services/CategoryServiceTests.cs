using AutoMapper;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Category;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Services;
using Lumiere.Domain.Entities;
using Lumiere.Tests.Unit.Helpers;
using Moq;

namespace Lumiere.Tests.Unit.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly IMapper _mapper;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _mapper = MapperFactory.Create();
        _uowMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
        _sut = new CategoryService(_uowMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Rings",    IsDeleted = false },
            new() { Id = 2, Name = "Necklaces",IsDeleted = false },
            new() { Id = 3, Name = "Deleted",  IsDeleted = true  }
        };
        _categoryRepoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(categories);

        var result = await _sut.GetAllAsync();

        result.Success.Should().BeTrue();
        result.Data!.Count.Should().Be(2); // deleted one is filtered out
        result.Data.Should().NotContain(c => c.Name == "Deleted");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ReturnsFail()
    {
        _categoryRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), default))
            .ReturnsAsync(true); // name already exists

        var result = await _sut.CreateAsync(new CreateCategoryRequest { Name = "Rings" });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already exists");
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_AutoGeneratesSlug_WhenSlugEmpty()
    {
        _categoryRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(), default))
            .ReturnsAsync(false);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        Category? savedCategory = null;
        _categoryRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Category>(), default))
            .Callback<Category, CancellationToken>((c, _) => savedCategory = c)
            .ReturnsAsync((Category c, CancellationToken _) => c);

        await _sut.CreateAsync(new CreateCategoryRequest { Name = "Fine Jewelry" });

        savedCategory!.Slug.Should().Be("fine-jewelry");
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_SoftDeletesIt()
    {
        var category = new Category { Id = 1, IsDeleted = false };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(category);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _sut.DeleteAsync(1);

        result.Success.Should().BeTrue();
        category.IsDeleted.Should().BeTrue();
        category.DeletedAt.Should().NotBeNull();
    }
}
