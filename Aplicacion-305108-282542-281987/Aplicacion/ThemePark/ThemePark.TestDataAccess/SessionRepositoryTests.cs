using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;

namespace ThemePark.TestDataAccess;

[TestClass]
public class SessionRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private SessionRepository _repository = null!;
    private Session _testSession = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _repository = new SessionRepository(_context, mockHttpContextAccessor.Object);

        _testSession = new Session
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = "test-token-123",
            CreatedAt = DateTime.Now,
            ExpirationDate = DateTime.Now.AddHours(1)
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddSessionToDatabase()
    {
        _repository.Add(_testSession);

        var savedSession = _context.Sessions.FirstOrDefault(s => s.Id == _testSession.Id);
        Assert.IsNotNull(savedSession);
        Assert.AreEqual(_testSession.Token, savedSession.Token);
    }

    [TestMethod]
    public void GetByToken_ShouldReturnSession_WhenTokenExists()
    {
        _repository.Add(_testSession);

        var result = _repository.GetByToken(_testSession.Token);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testSession.Token, result.Token);
        Assert.AreEqual(_testSession.UserId, result.UserId);
    }

    [TestMethod]
    public void GetByToken_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        var result = _repository.GetByToken("non-existent-token");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetCurrentUserId_ShouldReturnUserId_WhenUserIsAuthenticated()
    {
        var userId = Guid.NewGuid();
        var mockHttpContext = new Mock<HttpContext>();

        mockHttpContext.Setup(ctx => ctx.Items["UserId"]).Returns(userId);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

        var repository = new SessionRepository(_context, mockHttpContextAccessor.Object);

        var result = repository.GetCurrentUserId();

        Assert.AreEqual(userId, result);
    }

    [TestMethod]
    public void GetCurrentUserId_ShouldThrowUnauthorizedException_WhenUserIsNotAuthenticated()
    {
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(ctx => ctx.Items["UserId"]).Returns(null);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

        var repository = new SessionRepository(_context, mockHttpContextAccessor.Object);

        Assert.ThrowsException<UnauthorizedAccessException>(() => repository.GetCurrentUserId());
    }
}
