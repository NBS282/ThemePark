using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class VisitTests
{
    [TestMethod]
    public void Constructor_ShouldSetEntryProperties_WhenValidParametersProvided()
    {
        var userId = Guid.NewGuid();
        var attractionName = "Montaña Rusa";
        var entryTime = DateTime.Now;

        var visit = new Visit(userId, attractionName, entryTime);

        Assert.AreEqual(userId, visit.UserId);
        Assert.AreEqual(attractionName, visit.AttractionName);
        Assert.AreEqual(entryTime, visit.EntryTime);
    }

    [TestMethod]
    public void Constructor_ShouldSetDefaultValues_WhenValidParametersProvided()
    {
        var userId = Guid.NewGuid();
        var attractionName = "Montaña Rusa";
        var entryTime = DateTime.Now;

        var visit = new Visit(userId, attractionName, entryTime);

        Assert.IsNull(visit.ExitTime);
        Assert.IsTrue(visit.IsActive);
        Assert.AreNotEqual(Guid.Empty, visit.Id);
    }

    [TestMethod]
    public void Constructor_ShouldCreateVisitWithAttraction_WhenAttractionProvided()
    {
        var userId = Guid.NewGuid();
        var attraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now, 100);
        var entryTime = DateTime.Now;

        var visit = new Visit(userId, attraction, entryTime);

        Assert.AreEqual(attraction, visit.Attraction);
        Assert.AreEqual(attraction.Nombre, visit.AttractionName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void Constructor_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        var userId = Guid.Empty;
        var attractionName = "Montaña Rusa";
        var entryTime = DateTime.Now;

        new Visit(userId, attractionName, entryTime);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void Constructor_ShouldThrowArgumentException_WhenAttractionNameIsNull()
    {
        var userId = Guid.NewGuid();
        string attractionName = null!;
        var entryTime = DateTime.Now;

        new Visit(userId, attractionName, entryTime);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void Constructor_ShouldThrowArgumentException_WhenAttractionNameIsEmpty()
    {
        var userId = Guid.NewGuid();
        var attractionName = string.Empty;
        var entryTime = DateTime.Now;

        new Visit(userId, attractionName, entryTime);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void ConstructorWithAttraction_ShouldThrowArgumentException_WhenUserIdIsEmpty()
    {
        var userId = Guid.Empty;
        var attraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now, 100);
        var entryTime = DateTime.Now;

        new Visit(userId, attraction, entryTime);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void ConstructorWithAttraction_ShouldThrowArgumentException_WhenAttractionIsNull()
    {
        var userId = Guid.NewGuid();
        Attraction attraction = null!;
        var entryTime = DateTime.Now;

        new Visit(userId, attraction, entryTime);
    }

    [TestMethod]
    public void MarkExit_ShouldSetExitTimeAndDeactivateVisit()
    {
        var userId = Guid.NewGuid();
        var attractionName = "Montaña Rusa";
        var entryTime = DateTime.Now;
        var exitTime = DateTime.Now.AddHours(1);

        var visit = new Visit(userId, attractionName, entryTime);
        visit.MarkExit(exitTime);

        Assert.AreEqual(exitTime, visit.ExitTime);
        Assert.IsFalse(visit.IsActive);
    }

    [TestMethod]
    public void Constructor_ShouldSetScoringStrategyName_WhenProvided()
    {
        var userId = Guid.NewGuid();
        var attractionName = "Montaña Rusa";
        var entryTime = DateTime.Now;
        var strategyName = "Por Tiempo de Permanencia";

        var visit = new Visit(userId, attractionName, entryTime, 0, strategyName);

        Assert.AreEqual(strategyName, visit.ScoringStrategyName);
    }
}
