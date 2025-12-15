using ThemePark.Entities;

namespace ThemePark.TestDomain;

[TestClass]
public class SystemDateTimeTests
{
    private SystemDateTime _systemDateTime = null!;

    [TestInitialize]
    public void Setup()
    {
        _systemDateTime = new SystemDateTime();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _systemDateTime = null!;
    }

    [TestMethod]
    public void SystemDateTime_ShouldInitializeWithDefaultValues()
    {
        Assert.AreEqual(0, _systemDateTime.Id);
        Assert.IsNull(_systemDateTime.CurrentDateTime);
    }
}
