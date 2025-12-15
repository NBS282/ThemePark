using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.DataAccess.Utils;

namespace ThemePark.TestDataAccess;

[TestClass]
public class DateTimeRepositoryTests
{
    private readonly ThemeParkDbContext _context = DbContextBuilder.BuildTestDbContext();
    private readonly DateTimeRepository _repository;

    public DateTimeRepositoryTests()
    {
        _repository = new DateTimeRepository(_context);
    }

    [TestInitialize]
    public void Setup()
    {
        _context.Database.EnsureCreated();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
    }

    [TestMethod]
    public void GetCurrentDateTime_ShouldReturnNull_WhenNoCustomDateTimeSet()
    {
        var result = _repository.GetCurrentDateTime();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void SetCurrentDateTime_ShouldStoreDateTime()
    {
        var testDateTime = new DateTime(2024, 12, 25, 10, 30, 0);

        _repository.SetCurrentDateTime(testDateTime);
        var result = _repository.GetCurrentDateTime();

        Assert.AreEqual(testDateTime, result);
    }

    [TestMethod]
    public void SetCurrentDateTime_ShouldUpdateExistingDateTime_WhenDateTimeAlreadyExists()
    {
        var firstDateTime = new DateTime(2024, 12, 25, 10, 30, 0);
        var secondDateTime = new DateTime(2025, 1, 15, 14, 45, 0);

        _repository.SetCurrentDateTime(firstDateTime);
        var initialResult = _repository.GetCurrentDateTime();
        Assert.AreEqual(firstDateTime, initialResult);

        _repository.SetCurrentDateTime(secondDateTime);
        var updatedResult = _repository.GetCurrentDateTime();

        Assert.AreEqual(secondDateTime, updatedResult);
    }
}
