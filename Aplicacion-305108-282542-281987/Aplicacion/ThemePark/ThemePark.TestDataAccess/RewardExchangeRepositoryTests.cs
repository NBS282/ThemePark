using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.DataAccess.Utils;
using ThemePark.Entities;

namespace ThemePark.TestDataAccess;

[TestClass]
public class RewardExchangeRepositoryTests
{
    private readonly ThemeParkDbContext _context = DbContextBuilder.BuildTestDbContext();
    private readonly RewardExchangeRepository _repository;

    public RewardExchangeRepositoryTests()
    {
        _repository = new RewardExchangeRepository(_context);
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
    public void Save_ShouldStoreRewardExchange_WhenValidExchangeProvided()
    {
        var userId = Guid.NewGuid();
        var exchange = new RewardExchange(1, userId, 500, 1000);

        _repository.Save(exchange);

        var exchanges = _repository.GetByUserId(userId);
        Assert.AreEqual(1, exchanges.Count);
    }

    [TestMethod]
    public void GetByUserId_ShouldReturnUserExchanges()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var exchange1 = new RewardExchange(1, userId1, 500, 1000);
        var exchange2 = new RewardExchange(2, userId1, 300, 700);
        var exchange3 = new RewardExchange(3, userId2, 200, 800);
        _repository.Save(exchange1);
        _repository.Save(exchange2);
        _repository.Save(exchange3);

        var result = _repository.GetByUserId(userId1);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(e => e.UserId == userId1));
    }
}
