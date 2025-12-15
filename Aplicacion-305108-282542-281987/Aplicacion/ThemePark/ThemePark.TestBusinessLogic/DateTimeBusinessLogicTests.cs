using Moq;
using ThemePark.BusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class DateTimeBusinessLogicTests
{
    private Mock<IDateTimeRepository> _mockDateTimeRepository = null!;
    private DateTimeBusinessLogic _dateTimeBusinessLogic = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockDateTimeRepository = new Mock<IDateTimeRepository>(MockBehavior.Strict);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime())
            .Returns(new DateTime(2025, 9, 18, 15, 30, 0));

        _dateTimeBusinessLogic = new DateTimeBusinessLogic(_mockDateTimeRepository.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mockDateTimeRepository = null!;
        _dateTimeBusinessLogic = null!;
    }

    [TestMethod]
    public void GetCurrentDateTime_ShouldReturnFormattedDateTime_WhenCustomDateTimeSet()
    {
        var expectedString = "2025-09-18T15:30";

        var result = _dateTimeBusinessLogic.GetCurrentDateTime();

        Assert.AreEqual(expectedString, result);
        _mockDateTimeRepository.Verify(x => x.GetCurrentDateTime(), Times.AtLeastOnce);
    }

    [TestMethod]
    public void SetCurrentDateTime_ShouldUpdateDateTime_WhenValidDateTimeProvided()
    {
        var newDateTime = "2025-09-18T15:30";
        var expectedDateTime = new DateTime(2025, 9, 18, 15, 30, 0);
        _mockDateTimeRepository.Setup(x => x.SetCurrentDateTime(expectedDateTime));

        var result = _dateTimeBusinessLogic.SetCurrentDateTime(newDateTime);

        Assert.AreEqual(newDateTime, result);
        _mockDateTimeRepository.Verify(x => x.SetCurrentDateTime(expectedDateTime), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DateTimeBusinessLogicException))]
    public void SetCurrentDateTime_ShouldThrowDateTimeBusinessLogicException_WhenInvalidDateTimeFormatProvided()
    {
        var invalidDateTime = "2025-13-45T25:70";

        _dateTimeBusinessLogic.SetCurrentDateTime(invalidDateTime);
    }

    [TestMethod]
    [ExpectedException(typeof(DateTimeBusinessLogicException))]
    public void SetCurrentDateTime_ShouldThrowDateTimeBusinessLogicException_WhenDateTimeIsNull()
    {
        _dateTimeBusinessLogic.SetCurrentDateTime(string.Empty);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeSystemDateTime_WhenNoDateTimeExistsInDatabase()
    {
        var mockRepository = new Mock<IDateTimeRepository>(MockBehavior.Strict);

        mockRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        mockRepository.Setup(x => x.SetCurrentDateTime(It.IsAny<DateTime>()));

        var businessLogic = new DateTimeBusinessLogic(mockRepository.Object);

        mockRepository.Verify(x => x.GetCurrentDateTime(), Times.Once);

        mockRepository.Verify(x => x.SetCurrentDateTime(It.IsAny<DateTime>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DateTimeBusinessLogicException))]
    public void SetCurrentDateTime_ShouldThrowDateTimeBusinessLogicException_WhenDateTimeIsInThePast()
    {
        var currentDateTime = new DateTime(2025, 9, 18, 15, 30, 0);
        var pastDateTime = "2025-09-17T14:00";

        var mockRepository = new Mock<IDateTimeRepository>(MockBehavior.Strict);
        mockRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDateTime);
        var businessLogic = new DateTimeBusinessLogic(mockRepository.Object);

        businessLogic.SetCurrentDateTime(pastDateTime);
    }
}
