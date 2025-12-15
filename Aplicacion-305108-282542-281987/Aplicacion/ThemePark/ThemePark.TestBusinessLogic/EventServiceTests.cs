using Moq;
using ThemePark.BusinessLogic;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class EventServiceTests
{
    private Mock<IEventRepository> _mockRepository = null!;
    private Mock<IAttractionRepository> _mockAttractionRepository = null!;
    private IEventsService _service = null!;
    private Event _testEvent = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRepository = new Mock<IEventRepository>(MockBehavior.Strict);
        _mockAttractionRepository = new Mock<IAttractionRepository>(MockBehavior.Strict);
        _service = new EventService(_mockRepository.Object, _mockAttractionRepository.Object);

        _testEvent = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Evento Test",
            Fecha = DateTime.Today.AddDays(1),
            Hora = new TimeSpan(20, 0, 0),
            Aforo = 100,
            CostoAdicional = 50.0m
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockRepository.VerifyAll();
        _mockAttractionRepository.VerifyAll();
    }

    [TestMethod]
    public void GetAllEvents_ShouldReturnAllEventsFromRepository()
    {
        var expectedEvents = new List<Event> { _testEvent };
        _mockRepository.Setup(r => r.GetAll()).Returns(expectedEvents);

        var result = _service.GetAllEvents();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_testEvent.Id, result[0].Id);
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GetEventById_ShouldReturnSpecificEventFromRepository()
    {
        var eventId = _testEvent.Id.ToString();
        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);

        var result = _service.GetEventById(eventId);

        Assert.AreEqual(_testEvent, result);
        _mockRepository.Verify(r => r.GetById(_testEvent.Id), Times.Once);
    }

    [TestMethod]
    public void CreateEvent_ShouldCreateNewEventInRepository()
    {
        var nombre = "Nuevo Evento";
        var fecha = DateTime.Today.AddDays(2);
        var hora = new TimeSpan(19, 0, 0);
        var aforo = 150;
        var costoAdicional = 75.0m;
        var atracciones = new List<string> { "Montaña Rusa", "Simulador" };

        var atraccion1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Una montaña rusa extrema", DateTime.Now, 100);
        var atraccion2 = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 20, "Simulador de vuelo", DateTime.Now, 150);

        _mockRepository.Setup(r => r.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Simulador")).Returns(true);
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(atraccion1);
        _mockAttractionRepository.Setup(r => r.GetByName("Simulador")).Returns(atraccion2);
        _mockRepository.Setup(r => r.Add(It.IsAny<Event>())).Returns((Event e) => e);

        var result = _service.CreateEvent(nombre, fecha, hora, aforo, costoAdicional, atracciones);

        Assert.IsNotNull(result);
        _mockRepository.Verify(r => r.ExistsByName(nombre), Times.Once);
        _mockRepository.Verify(r => r.Add(It.IsAny<Event>()), Times.Once);
    }

    [TestMethod]
    public void DeleteEvent_ShouldDeleteEventFromRepository()
    {
        var eventId = _testEvent.Id.ToString();
        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);
        _mockRepository.Setup(r => r.Delete(_testEvent.Id));

        _service.DeleteEvent(eventId);

        _mockRepository.Verify(r => r.GetById(_testEvent.Id), Times.Once);
        _mockRepository.Verify(r => r.Delete(_testEvent.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateEvent_ShouldThrowBusinessLogicException_WhenEventNameAlreadyExists()
    {
        var nombre = "Evento Existente";
        var fecha = DateTime.Today.AddDays(2);
        var hora = new TimeSpan(19, 0, 0);
        var aforo = 150;
        var costoAdicional = 75.0m;
        var atracciones = new List<string>();

        _mockRepository.Setup(r => r.ExistsByName(nombre)).Returns(true);

        _service.CreateEvent(nombre, fecha, hora, aforo, costoAdicional, atracciones);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetEventById_ShouldThrowKeyNotFoundException_WhenEventDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var eventId = nonExistentId.ToString();

        _mockRepository.Setup(r => r.GetById(nonExistentId))
            .Throws(new KeyNotFoundException($"Event with ID '{nonExistentId}' not found"));

        _service.GetEventById(eventId);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void DeleteEvent_ShouldThrowKeyNotFoundException_WhenEventDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var eventId = nonExistentId.ToString();

        _mockRepository.Setup(r => r.GetById(nonExistentId))
            .Throws(new KeyNotFoundException($"Event with ID '{nonExistentId}' not found"));

        _service.DeleteEvent(eventId);
    }

    [TestMethod]
    public void CreateEvent_ShouldAssignAttractions_WhenAtraccionesIncluidasProvided()
    {
        var nombre = "Evento con Atracciones";
        var fecha = DateTime.Today.AddDays(2);
        var hora = new TimeSpan(19, 0, 0);
        var aforo = 150;
        var costoAdicional = 75.0m;
        var nombreAtracciones = new List<string> { "Montaña Rusa", "Simulador" };

        var atraccion1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Una montaña rusa extrema", DateTime.Now, 100);
        var atraccion2 = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 20, "Simulador de vuelo", DateTime.Now, 150);

        Event? capturedEvent = null;

        _mockRepository.Setup(r => r.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Simulador")).Returns(true);
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(atraccion1);
        _mockAttractionRepository.Setup(r => r.GetByName("Simulador")).Returns(atraccion2);
        _mockRepository.Setup(r => r.Add(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEvent = e)
            .Returns((Event e) => e);

        var result = _service.CreateEvent(nombre, fecha, hora, aforo, costoAdicional, nombreAtracciones);

        Assert.IsNotNull(capturedEvent);
        Assert.AreEqual(2, capturedEvent.Atracciones.Count);
        Assert.IsTrue(capturedEvent.Atracciones.Any(a => a.Nombre == "Montaña Rusa"));
    }

    [TestMethod]
    public void CreateEvent_ShouldIncludeAllAttractions_WhenAtraccionesIncluidasProvided()
    {
        var nombre = "Evento con Atracciones";
        var fecha = DateTime.Today.AddDays(2);
        var hora = new TimeSpan(19, 0, 0);
        var aforo = 150;
        var costoAdicional = 75.0m;
        var nombreAtracciones = new List<string> { "Montaña Rusa", "Simulador" };

        var atraccion1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Una montaña rusa extrema", DateTime.Now, 100);
        var atraccion2 = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 20, "Simulador de vuelo", DateTime.Now, 150);

        Event? capturedEvent = null;

        _mockRepository.Setup(r => r.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Simulador")).Returns(true);
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(atraccion1);
        _mockAttractionRepository.Setup(r => r.GetByName("Simulador")).Returns(atraccion2);
        _mockRepository.Setup(r => r.Add(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEvent = e)
            .Returns((Event e) => e);

        var result = _service.CreateEvent(nombre, fecha, hora, aforo, costoAdicional, nombreAtracciones);

        Assert.IsTrue(capturedEvent!.Atracciones.Any(a => a.Nombre == "Simulador"));
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateEvent_ShouldThrowBusinessLogicException_WhenAttractionDoesNotExist()
    {
        var nombre = "Evento Test";
        var fecha = DateTime.Today.AddDays(2);
        var hora = new TimeSpan(19, 0, 0);
        var aforo = 150;
        var costoAdicional = 75.0m;
        var atracciones = new List<string> { "Montaña Rusa", "AtraccionInexistente" };

        _mockRepository.Setup(r => r.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.ExistsByName("AtraccionInexistente")).Returns(false);

        _service.CreateEvent(nombre, fecha, hora, aforo, costoAdicional, atracciones);
    }

    [TestMethod]
    public void UpdateEvent_ShouldUpdateEventInRepository()
    {
        var eventId = _testEvent.Id.ToString();
        var nuevoNombre = "Evento Actualizado";
        var nuevaFecha = DateTime.Today.AddDays(3);
        var nuevaHora = new TimeSpan(21, 0, 0);
        var nuevoAforo = 200;
        var nuevoCosto = 100.0m;
        var atracciones = new List<string> { "Montaña Rusa" };

        var atraccion1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Una montaña rusa extrema", DateTime.Now, 100);

        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);
        _mockRepository.Setup(r => r.ExistsByName(nuevoNombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(atraccion1);
        _mockRepository.Setup(r => r.Update(It.IsAny<Event>())).Returns((Event e) => e);

        var result = _service.UpdateEvent(eventId, nuevoNombre, nuevaFecha, nuevaHora, nuevoAforo, nuevoCosto, atracciones);

        Assert.IsNotNull(result);
        Assert.AreEqual(nuevoNombre, result.Name);
        Assert.AreEqual(nuevaFecha, result.Fecha);
        Assert.AreEqual(nuevaHora, result.Hora);
        Assert.AreEqual(nuevoAforo, result.Aforo);
        Assert.AreEqual(nuevoCosto, result.CostoAdicional);
        _mockRepository.Verify(r => r.Update(It.IsAny<Event>()), Times.Once);
    }

    [TestMethod]
    public void UpdateEvent_ShouldAllowSameName_WhenUpdatingSameEvent()
    {
        var eventId = _testEvent.Id.ToString();
        var mismoNombre = _testEvent.Name;
        var nuevaFecha = DateTime.Today.AddDays(3);
        var nuevaHora = new TimeSpan(21, 0, 0);
        var nuevoAforo = 200;
        var nuevoCosto = 100.0m;
        var atracciones = new List<string> { "Montaña Rusa" };

        var atraccion1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Una montaña rusa extrema", DateTime.Now, 100);

        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);
        _mockRepository.Setup(r => r.ExistsByName(mismoNombre)).Returns(true);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(atraccion1);
        _mockRepository.Setup(r => r.Update(It.IsAny<Event>())).Returns((Event e) => e);

        var result = _service.UpdateEvent(eventId, mismoNombre, nuevaFecha, nuevaHora, nuevoAforo, nuevoCosto, atracciones);

        Assert.IsNotNull(result);
        _mockRepository.Verify(r => r.Update(It.IsAny<Event>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void UpdateEvent_ShouldThrowBusinessLogicException_WhenNewNameAlreadyExists()
    {
        var eventId = _testEvent.Id.ToString();
        var nombreExistente = "Otro Evento Existente";
        var nuevaFecha = DateTime.Today.AddDays(3);
        var nuevaHora = new TimeSpan(21, 0, 0);
        var nuevoAforo = 200;
        var nuevoCosto = 100.0m;
        var atracciones = new List<string> { "Montaña Rusa" };

        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);
        _mockRepository.Setup(r => r.ExistsByName(nombreExistente)).Returns(true);

        _service.UpdateEvent(eventId, nombreExistente, nuevaFecha, nuevaHora, nuevoAforo, nuevoCosto, atracciones);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void UpdateEvent_ShouldThrowBusinessLogicException_WhenAttractionDoesNotExist()
    {
        var eventId = _testEvent.Id.ToString();
        var nuevoNombre = "Evento Actualizado";
        var nuevaFecha = DateTime.Today.AddDays(3);
        var nuevaHora = new TimeSpan(21, 0, 0);
        var nuevoAforo = 200;
        var nuevoCosto = 100.0m;
        var atracciones = new List<string> { "AtraccionInexistente" };

        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);
        _mockRepository.Setup(r => r.ExistsByName(nuevoNombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("AtraccionInexistente")).Returns(false);

        _service.UpdateEvent(eventId, nuevoNombre, nuevaFecha, nuevaHora, nuevoAforo, nuevoCosto, atracciones);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateEvent_ShouldThrowKeyNotFoundException_WhenEventDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var eventId = nonExistentId.ToString();
        var nuevoNombre = "Evento Actualizado";
        var nuevaFecha = DateTime.Today.AddDays(3);
        var nuevaHora = new TimeSpan(21, 0, 0);
        var nuevoAforo = 200;
        var nuevoCosto = 100.0m;
        var atracciones = new List<string> { "Montaña Rusa" };

        _mockRepository.Setup(r => r.GetById(nonExistentId))
            .Throws(new KeyNotFoundException($"Event with ID '{nonExistentId}' not found"));

        _service.UpdateEvent(eventId, nuevoNombre, nuevaFecha, nuevaHora, nuevoAforo, nuevoCosto, atracciones);
    }

    [TestMethod]
    public void UpdateEvent_ShouldUpdateAttractions_WhenAtraccionesIncluidasProvided()
    {
        var eventId = _testEvent.Id.ToString();
        var nuevoNombre = "Evento con Nuevas Atracciones";
        var nuevaFecha = DateTime.Today.AddDays(3);
        var nuevaHora = new TimeSpan(21, 0, 0);
        var nuevoAforo = 200;
        var nuevoCosto = 100.0m;
        var nombreAtracciones = new List<string> { "Montaña Rusa", "Simulador" };

        var atraccion1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "Una montaña rusa extrema", DateTime.Now, 100);
        var atraccion2 = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 20, "Simulador de vuelo", DateTime.Now, 150);

        Event? capturedEvent = null;

        _mockRepository.Setup(r => r.GetById(_testEvent.Id)).Returns(_testEvent);
        _mockRepository.Setup(r => r.ExistsByName(nuevoNombre)).Returns(false);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Montaña Rusa")).Returns(true);
        _mockAttractionRepository.Setup(r => r.ExistsByName("Simulador")).Returns(true);
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(atraccion1);
        _mockAttractionRepository.Setup(r => r.GetByName("Simulador")).Returns(atraccion2);
        _mockRepository.Setup(r => r.Update(It.IsAny<Event>()))
            .Callback<Event>(e => capturedEvent = e)
            .Returns((Event e) => e);

        var result = _service.UpdateEvent(eventId, nuevoNombre, nuevaFecha, nuevaHora, nuevoAforo, nuevoCosto, nombreAtracciones);

        Assert.IsNotNull(capturedEvent);
        Assert.AreEqual(2, capturedEvent.Atracciones.Count);
        Assert.IsTrue(capturedEvent.Atracciones.Any(a => a.Nombre == "Montaña Rusa"));
        Assert.IsTrue(capturedEvent.Atracciones.Any(a => a.Nombre == "Simulador"));
    }
}
