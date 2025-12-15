using Microsoft.AspNetCore.Mvc;
using Moq;
using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Attractions;
using ThemeParkApi.Controllers;

namespace ThemePark.TestAPI;

[TestClass]
public class MaintenancesControllerTests
{
    private Mock<IAttractionsBusinessLogic> _mockAttractionsBusinessLogic = null!;
    private MaintenancesController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAttractionsBusinessLogic = new Mock<IAttractionsBusinessLogic>(MockBehavior.Strict);
        _controller = new MaintenancesController(_mockAttractionsBusinessLogic.Object);
    }

    [TestMethod]
    public void CreateMaintenance_ShouldReturnCreated_WhenValidRequestProvided()
    {
        var attractionName = "Montaña Rusa T-Rex";
        var request = new CreateMaintenanceRequest
        {
            Fecha = "2027-11-01",
            HoraInicio = "09:00",
            DuracionMinutos = 120,
            Descripcion = "Mantenimiento preventivo del sistema de frenos"
        };

        var expectedMaintenanceId = Guid.NewGuid().ToString();
        var expectedIncidentId = Guid.NewGuid().ToString();

        _mockAttractionsBusinessLogic
            .Setup(x => x.CreatePreventiveMaintenance(It.IsAny<Maintenance>()))
            .Returns((expectedMaintenanceId, expectedIncidentId));

        var result = _controller.CreateMaintenance(attractionName, request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = (CreatedResult)result;
        var response = createdResult.Value as MaintenanceResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(expectedMaintenanceId, response.Id);
    }

    [TestMethod]
    public void GetMaintenances_ShouldReturnOk_WhenMaintenancesExist()
    {
        var attractionName = "Montaña Rusa T-Rex";
        var maintenance = new Maintenance(attractionName, new System.DateTime(2027, 11, 1), new System.TimeSpan(9, 0, 0), 120, "Mantenimiento preventivo");

        var maintenances = new List<Maintenance> { maintenance };

        _mockAttractionsBusinessLogic
            .Setup(x => x.GetMaintenancesByAttraction(attractionName))
            .Returns(maintenances);

        var result = _controller.GetMaintenances(attractionName);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = okResult.Value as List<MaintenanceResponseModel>;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Count);
    }

    [TestMethod]
    public void DeleteMaintenance_ShouldReturnNoContent_WhenMaintenanceIsCanceled()
    {
        var attractionName = "Montaña Rusa T-Rex";
        var maintenanceId = Guid.NewGuid().ToString();

        _mockAttractionsBusinessLogic
            .Setup(x => x.CancelPreventiveMaintenance(attractionName, maintenanceId))
            .Verifiable();

        var result = _controller.DeleteMaintenance(attractionName, maintenanceId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _mockAttractionsBusinessLogic.Verify(x => x.CancelPreventiveMaintenance(attractionName, maintenanceId), Times.Once);
    }
}
