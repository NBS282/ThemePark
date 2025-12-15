using Moq;
using ThemePark.BusinessLogic;
using ThemePark.Entities;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<ISessionRepository> _mockSessionRepository = null!;
    private IAuthService _authService = null!;
    private User _testUser = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockSessionRepository = new Mock<ISessionRepository>(MockBehavior.Strict);
        _authService = new AuthService(_mockUserRepository.Object, _mockSessionRepository.Object);

        _testUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Nombre = "Test User"
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockUserRepository = null!;
        _mockSessionRepository = null!;
        _authService = null!;
        _testUser = null!;
    }

    [TestMethod]
    public void Login_ShouldReturnValidSession_WhenCredentialsAreValid()
    {
        var email = "test@example.com";
        var password = "password123";

        _mockUserRepository.Setup(r => r.GetByEmailAndPassword(email, password))
                          .Returns(_testUser);
        _mockSessionRepository.Setup(r => r.Add(It.IsAny<Session>()));

        var (session, user) = _authService.Login(email, password);

        Assert.IsNotNull(session);
        Assert.IsNotNull(session.Token);
        Assert.AreNotEqual(string.Empty, session.Token);
    }

    [TestMethod]
    public void Login_ShouldReturnUserWithCorrectId_WhenCredentialsAreValid()
    {
        var email = "test@example.com";
        var password = "password123";

        _mockUserRepository.Setup(r => r.GetByEmailAndPassword(email, password))
                          .Returns(_testUser);
        _mockSessionRepository.Setup(r => r.Add(It.IsAny<Session>()));

        var (session, user) = _authService.Login(email, password);

        Assert.AreEqual(_testUser.Id, session.UserId);
        Assert.IsNotNull(user);
        Assert.AreEqual(_testUser.Id, user.Id);
    }

    [TestMethod]
    public void ValidateToken_ShouldReturnTrue_WhenTokenIsValid()
    {
        var validToken = "valid-token-123";
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = _testUser.Id,
            Token = validToken,
            ExpirationDate = DateTime.Now.AddHours(1)
        };

        _mockSessionRepository.Setup(r => r.GetByToken(validToken))
                            .Returns(session);

        var result = _authService.ValidateToken(validToken);

        Assert.IsTrue(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCredentialsException))]
    public void Login_ShouldThrowException_WhenCredentialsAreInvalid()
    {
        var email = "invalid@example.com";
        var password = "wrongpassword";

        _mockUserRepository.Setup(r => r.GetByEmailAndPassword(email, password))
                          .Returns((User?)null);

        _authService.Login(email, password);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAuthenticationException))]
    public void Login_ShouldThrowException_WhenEmailIsEmpty()
    {
        var emptyEmail = string.Empty;
        var password = "password123";

        _authService.Login(emptyEmail, password);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAuthenticationException))]
    public void Login_ShouldThrowException_WhenPasswordIsEmpty()
    {
        var email = "test@example.com";
        var emptyPassword = string.Empty;

        _authService.Login(email, emptyPassword);
    }

    [TestMethod]
    public void ValidateToken_ShouldReturnFalse_WhenTokenIsEmpty()
    {
        var emptyToken = string.Empty;

        var result = _authService.ValidateToken(emptyToken);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetUserByToken_ShouldReturnUser_WhenTokenIsValid()
    {
        var validToken = "valid-token-123";
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = _testUser.Id,
            Token = validToken,
            ExpirationDate = DateTime.Now.AddHours(1)
        };

        _mockSessionRepository.Setup(r => r.GetByToken(validToken))
                            .Returns(session);
        _mockUserRepository.Setup(r => r.GetById(_testUser.Id))
                          .Returns(_testUser);

        var result = _authService.GetUserByToken(validToken);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
    }

    [TestMethod]
    public void ValidateToken_ShouldReturnFalse_WhenTokenIsExpired()
    {
        var expiredToken = "expired-token-123";
        var expiredSession = new Session
        {
            Id = Guid.NewGuid(),
            UserId = _testUser.Id,
            Token = expiredToken,
            ExpirationDate = DateTime.Now.AddHours(-1)
        };

        _mockSessionRepository.Setup(r => r.GetByToken(expiredToken))
                            .Returns(expiredSession);

        var result = _authService.ValidateToken(expiredToken);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ValidateToken_ShouldReturnFalse_WhenTokenIsNull()
    {
        var result = _authService.ValidateToken(null);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetUserByToken_ShouldReturnNull_WhenTokenIsInvalid()
    {
        var invalidToken = "invalid-token-123";

        _mockSessionRepository.Setup(r => r.GetByToken(invalidToken))
                            .Returns((Session?)null);

        var result = _authService.GetUserByToken(invalidToken);

        Assert.IsNull(result);
        _mockUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void GetUserByToken_ShouldReturnNull_WhenTokenIsExpired()
    {
        var expiredToken = "expired-token-456";
        var expiredSession = new Session
        {
            Id = Guid.NewGuid(),
            UserId = _testUser.Id,
            Token = expiredToken,
            ExpirationDate = DateTime.Now.AddHours(-2)
        };

        _mockSessionRepository.Setup(r => r.GetByToken(expiredToken))
                            .Returns(expiredSession);

        var result = _authService.GetUserByToken(expiredToken);

        Assert.IsNull(result);
        _mockUserRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }
}
