using AutoMapper;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Services;
using Lumiere.Domain.Entities;
using Lumiere.Tests.Unit.Helpers;
using Moq;

namespace Lumiere.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly IMapper _mapper;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _mapper = MapperFactory.Create();
        _uowMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _sut = new UserService(_uowMock.Object, _mapper);
    }

 
}
