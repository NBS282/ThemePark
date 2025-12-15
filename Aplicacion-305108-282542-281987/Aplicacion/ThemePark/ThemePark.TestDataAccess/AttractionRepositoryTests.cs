using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.DataAccess.Utils;
using ThemePark.Entities;
using ThemePark.Enums;

namespace ThemePark.TestDataAccess;

[TestClass]
public class AttractionRepositoryTests
{
    private readonly ThemeParkDbContext _context = DbContextBuilder.BuildTestDbContext();
    private readonly AttractionRepository _repository;

    public AttractionRepositoryTests()
    {
        _repository = new AttractionRepository(_context);
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
    public void ExistsByName_ShouldReturnFalse_WhenAttractionDoesNotExist()
    {
        var nombre = "Atraccion Inexistente";

        var result = _repository.ExistsByName(nombre);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Save_ShouldStoreAttraction_WhenValidAttractionProvided()
    {
        var attraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante", DateTime.Now);

        _repository.Save(attraction);

        var exists = _repository.ExistsByName("Montaña Rusa");
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void GetByName_ShouldReturnAttraction_WhenAttractionExists()
    {
        var originalAttraction = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 30, "Tecnología avanzada", DateTime.Now);
        _repository.Save(originalAttraction);

        Attraction result = _repository.GetByName("Simulador");

        Assert.AreEqual("Simulador", result.Nombre);
        Assert.AreEqual(TipoAtraccion.Simulador, result.Tipo);
        Assert.AreEqual(8, result.EdadMinima);
    }

    [TestMethod]
    public void GetByName_ShouldReturnAttractionWithCorrectCapacityAndDescription_WhenAttractionExists()
    {
        var originalAttraction = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 30, "Tecnología avanzada", DateTime.Now);
        _repository.Save(originalAttraction);

        Attraction result = _repository.GetByName("Simulador");

        Assert.AreEqual(30, result.CapacidadMaxima);
        Assert.AreEqual("Tecnología avanzada", result.Descripcion);
    }

    [TestMethod]
    public void Delete_ShouldRemoveAttraction_WhenAttractionExists()
    {
        var attraction = new Attraction("Casa Embrujada", TipoAtraccion.Espectaculo, 16, 20, "Experiencia aterradora", DateTime.Now);
        _repository.Save(attraction);

        _repository.Delete("Casa Embrujada");

        var exists = _repository.ExistsByName("Casa Embrujada");
        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void Save_ShouldUpdateAttraction_WhenAttractionAlreadyExists()
    {
        var originalAttraction = new Attraction("Carrusel", TipoAtraccion.ZonaInteractiva, 5, 40, "Descripción original", DateTime.Now);
        _repository.Save(originalAttraction);

        var updatedAttraction = new Attraction("Carrusel", TipoAtraccion.ZonaInteractiva, 3, 50, "Descripción actualizada", DateTime.Now);

        _repository.Save(updatedAttraction);

        var result = _repository.GetByName("Carrusel");
        Assert.AreEqual("Carrusel", result.Nombre);
        Assert.AreEqual(3, result.EdadMinima);
    }

    [TestMethod]
    public void Save_ShouldUpdateAttractionCapacityAndDescription_WhenAttractionAlreadyExists()
    {
        var originalAttraction = new Attraction("Carrusel", TipoAtraccion.MontañaRusa, 5, 40, "Descripción original", DateTime.Now);
        _repository.Save(originalAttraction);

        var updatedAttraction = new Attraction("Carrusel", TipoAtraccion.MontañaRusa, 3, 50, "Descripción actualizada", DateTime.Now);

        _repository.Save(updatedAttraction);

        var result = _repository.GetByName("Carrusel");
        Assert.AreEqual(50, result.CapacidadMaxima);
        Assert.AreEqual("Descripción actualizada", result.Descripcion);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllAttractions_WhenAttractionsExist()
    {
        var attraction1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante", DateTime.Now);
        var attraction2 = new Attraction("Carrusel", TipoAtraccion.ZonaInteractiva, 5, 40, "Tranquila", DateTime.Now);
        var attraction3 = new Attraction("Simulador", TipoAtraccion.Simulador, 8, 30, "Tecnología avanzada", DateTime.Now);

        _repository.Save(attraction1);
        _repository.Save(attraction2);
        _repository.Save(attraction3);

        var result = _repository.GetAll();

        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.Any(a => a.Nombre == "Montaña Rusa"));
        Assert.IsTrue(result.Any(a => a.Nombre == "Carrusel"));
        Assert.IsTrue(result.Any(a => a.Nombre == "Simulador"));
    }

    [TestMethod]
    public void GetById_ShouldReturnAttraction_WhenAttractionExists()
    {
        var attraction = new Attraction("Casa Embrujada", TipoAtraccion.Espectaculo, 16, 20, "Experiencia aterradora", DateTime.Now);
        _repository.Save(attraction);

        var result = _repository.GetById("Casa Embrujada");

        Assert.IsNotNull(result);
        Assert.AreEqual("Casa Embrujada", result.Nombre);
        Assert.AreEqual(TipoAtraccion.Espectaculo, result.Tipo);
        Assert.AreEqual(16, result.EdadMinima);
        Assert.AreEqual(20, result.CapacidadMaxima);
    }
}
