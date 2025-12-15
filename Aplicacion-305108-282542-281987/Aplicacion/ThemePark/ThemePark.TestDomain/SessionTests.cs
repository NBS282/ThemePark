using ThemePark.Entities;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class SessionTests
{
    private Session _session = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _session = new Session();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _session = null!;
    }

    [TestMethod]
    public void Session_ShouldInitializeWithValidUserId()
    {
        _session.UserId = Guid.NewGuid();
        Assert.AreNotEqual(Guid.Empty, _session.UserId);
    }

    [TestMethod]
    public void Session_ShouldInitializeWithEmptyToken()
    {
        Assert.AreEqual(string.Empty, _session.Token);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void Session_ShouldThrowException_WhenTokenIsEmpty()
    {
        _session.Token = string.Empty;
    }

    [TestMethod]
    public void Session_ShouldInitializeWithDefaultExpirationDate()
    {
        Assert.AreEqual(DateTime.MinValue, _session.ExpirationDate);
    }

    [TestMethod]
    public void Session_ShouldInitializeWithDefaultCreatedAt()
    {
        Assert.AreEqual(DateTime.MinValue, _session.CreatedAt);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void Session_ShouldThrowException_WhenCreatedAtIsAfterExpirationDate()
    {
        var futureDate = DateTime.Now.AddDays(1);
        var pastDate = DateTime.Now;

        _session.ExpirationDate = pastDate;
        _session.CreatedAt = futureDate;
    }

    [TestMethod]
    public void Session_ShouldInitializeWithEmptyId()
    {
        Assert.AreEqual(Guid.Empty, _session.Id);
    }
}
