using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class AttractionTests
{
    [TestMethod]
    public void Constructor_ShouldSetBasicProperties_WhenValidPropertiesProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var tipo = TipoAtraccion.MontañaRusa;
        var edadMinima = 12;
        var capacidadMaxima = 50;
        var descripcion = "Emocionante montaña rusa temática de dinosaurios";
        var fechaCreacion = new DateTime(2025, 9, 18, 14, 30, 0);

        var attraction = new Attraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, fechaCreacion);

        Assert.AreEqual(nombre, attraction.Nombre);
        Assert.AreEqual(tipo, attraction.Tipo);
        Assert.AreEqual(edadMinima, attraction.EdadMinima);
    }

    [TestMethod]
    public void Constructor_ShouldSetCapacityAndDescriptionProperties_WhenValidPropertiesProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var tipo = TipoAtraccion.MontañaRusa;
        var edadMinima = 12;
        var capacidadMaxima = 50;
        var descripcion = "Emocionante montaña rusa temática de dinosaurios";
        var fechaCreacion = new DateTime(2025, 9, 18, 14, 30, 0);

        var attraction = new Attraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, fechaCreacion);

        Assert.AreEqual(capacidadMaxima, attraction.CapacidadMaxima);
        Assert.AreEqual(descripcion, attraction.Descripcion);
        Assert.AreEqual(fechaCreacion, attraction.FechaCreacion);
    }

    [TestMethod]
    public void Constructor_ShouldSetDefaultValues_WhenValidPropertiesProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var tipo = TipoAtraccion.MontañaRusa;
        var edadMinima = 12;
        var capacidadMaxima = 50;
        var descripcion = "Emocionante montaña rusa temática de dinosaurios";
        var fechaCreacion = new DateTime(2025, 9, 18, 14, 30, 0);

        var attraction = new Attraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, fechaCreacion);

        Assert.IsNull(attraction.FechaModificacion);
        Assert.AreEqual(0, attraction.AforoActual);
        Assert.IsFalse(attraction.TieneIncidenciaActiva);
    }

    [TestMethod]
    public void Constructor_ShouldSetPoints_WhenPointsValueProvided()
    {
        var nombre = "Simulador VR";
        var tipo = TipoAtraccion.Simulador;
        var edadMinima = 8;
        var capacidadMaxima = 20;
        var descripcion = "Simulador de realidad virtual";
        var fechaCreacion = DateTime.Now;
        var points = 150;

        var attraction = new Attraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, fechaCreacion, points);

        Assert.AreEqual(points, attraction.Points);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void Constructor_ShouldThrowArgumentException_WhenNombreIsEmpty()
    {
        new Attraction(string.Empty, TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void Constructor_ShouldThrowArgumentException_WhenEdadMinimaIsNegative()
    {
        new Attraction("Test", TipoAtraccion.MontañaRusa, -1, 50, "Descripcion", DateTime.Now);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void Constructor_ShouldThrowArgumentException_WhenCapacidadMaximaIsZeroOrNegative()
    {
        new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 0, "Descripcion", DateTime.Now);
    }

    [TestMethod]
    public void IncrementarAforo_ShouldIncreaseAforoActual_WhenCalled()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);

        attraction.IncrementarAforo();

        Assert.AreEqual(1, attraction.AforoActual);
    }

    [TestMethod]
    [ExpectedException(typeof(CapacityExceededException))]
    public void IncrementarAforo_ShouldThrowCapacityExceededException_WhenCapacityIsReached()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 2, "Descripcion", DateTime.Now);
        attraction.IncrementarAforo();
        attraction.IncrementarAforo();

        attraction.IncrementarAforo();
    }

    [TestMethod]
    public void DecrementarAforo_ShouldDecreaseAforoActual_WhenAforoIsGreaterThanZero()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);
        attraction.IncrementarAforo();
        attraction.IncrementarAforo();

        attraction.DecrementarAforo();

        Assert.AreEqual(1, attraction.AforoActual);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void DecrementarAforo_ShouldThrowInvalidAttractionDataException_WhenAforoIsZero()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);

        attraction.DecrementarAforo();
    }

    [TestMethod]
    public void ActivarIncidencia_ShouldSetTieneIncidenciaActivaToTrue_WhenCalled()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);

        attraction.ActivarIncidencia();

        Assert.IsTrue(attraction.TieneIncidenciaActiva);
    }

    [TestMethod]
    public void DesactivarIncidencia_ShouldSetTieneIncidenciaActivaToFalse_WhenCalled()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);
        attraction.ActivarIncidencia();

        attraction.DesactivarIncidencia();

        Assert.IsFalse(attraction.TieneIncidenciaActiva);
    }

    [TestMethod]
    public void UpdateInfo_ShouldUpdateDescription_WhenDescriptionProvided()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion Original", DateTime.Now);
        var newDescription = "Nueva Descripcion";

        attraction.UpdateInfo(descripcion: newDescription);

        Assert.AreEqual(newDescription, attraction.Descripcion);
        Assert.IsNotNull(attraction.FechaModificacion);
    }

    [TestMethod]
    public void UpdateInfo_ShouldUpdateCapacidadMaxima_WhenValidCapacidadProvided()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);
        var newCapacidad = 100;

        attraction.UpdateInfo(capacidadMaxima: newCapacidad);

        Assert.AreEqual(newCapacidad, attraction.CapacidadMaxima);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void UpdateInfo_ShouldThrowException_WhenCapacidadMaximaIsNegative()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);

        attraction.UpdateInfo(capacidadMaxima: -10);
    }

    [TestMethod]
    public void UpdateInfo_ShouldUpdateEdadMinima_WhenValidEdadProvided()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);
        var newEdad = 18;

        attraction.UpdateInfo(edadMinima: newEdad);

        Assert.AreEqual(newEdad, attraction.EdadMinima);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidAttractionDataException))]
    public void UpdateInfo_ShouldThrowException_WhenEdadMinimaIsNegative()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);

        attraction.UpdateInfo(edadMinima: -1);
    }

    [TestMethod]
    public void UpdateInfo_ShouldSetCustomFechaModificacion_WhenFechaModificacionProvided()
    {
        var attraction = new Attraction("Test", TipoAtraccion.MontañaRusa, 12, 50, "Descripcion", DateTime.Now);
        var customDate = new DateTime(2025, 12, 25);

        attraction.UpdateInfo(fechaModificacion: customDate);

        Assert.AreEqual(customDate, attraction.FechaModificacion);
    }
}
