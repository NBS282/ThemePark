using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class UserTests
{
    private User _user = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _user = new User();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _user = null!;
    }

    [TestMethod]
    public void User_ShouldInitializeWithEmptyId()
    {
        Assert.AreEqual(Guid.Empty, _user.Id);
    }

    [TestMethod]
    public void User_ShouldInitializeWithEmptyNombre()
    {
        Assert.AreEqual(string.Empty, _user.Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenNombreIsEmpty()
    {
        _user.Nombre = string.Empty;
    }

    [TestMethod]
    public void User_ShouldSetValidNombre()
    {
        _user.Nombre = "Juan";
        Assert.AreEqual("Juan", _user.Nombre);
    }

    [TestMethod]
    public void User_ShouldInitializeWithEmptyEmail()
    {
        Assert.AreEqual(string.Empty, _user.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenEmailFormatIsInvalid()
    {
        _user.Email = "invalid-email";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenEmailIsEmpty()
    {
        _user.Email = string.Empty;
    }

    [TestMethod]
    public void User_ShouldSetValidEmail()
    {
        _user.Email = "test@example.com";
        Assert.AreEqual("test@example.com", _user.Email);
    }

    [TestMethod]
    public void User_ShouldInitializeWithEmptyApellido()
    {
        Assert.AreEqual(string.Empty, _user.Apellido);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenApellidoIsEmpty()
    {
        _user.Apellido = string.Empty;
    }

    [TestMethod]
    public void User_ShouldSetValidApellido()
    {
        _user.Apellido = "Pérez";
        Assert.AreEqual("Pérez", _user.Apellido);
    }

    [TestMethod]
    public void User_ShouldInitializeWithEmptyContraseña()
    {
        Assert.AreEqual(string.Empty, _user.Contraseña);
    }

    [TestMethod]
    public void User_ShouldInitializeWithDefaultFechaNacimiento()
    {
        Assert.AreEqual(DateTime.MinValue, _user.FechaNacimiento);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenFechaNacimientoIsInFuture()
    {
        _user.FechaNacimiento = DateTime.Now.AddDays(1);
    }

    [TestMethod]
    public void User_ShouldInitializeWithDefaultNivelMembresia()
    {
        var userWithVisitante = new User([new RolVisitante()]);
        Assert.AreEqual(NivelMembresia.Estándar, userWithVisitante.GetMembresia());
    }

    [TestMethod]
    public void User_ShouldInitializeWithDefaultRoles()
    {
        Assert.IsNotNull(_user.Roles);
        Assert.AreEqual(0, _user.Roles.Count);
    }

    [TestMethod]
    public void User_ShouldInitializeWithDefaultFechaRegistro()
    {
        Assert.AreEqual(DateTime.MinValue, _user.FechaRegistro);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenContraseñaIsEmpty()
    {
        _user.Contraseña = string.Empty;
    }

    [TestMethod]
    public void User_ShouldSetValidContraseña()
    {
        _user.Contraseña = "password123";
        Assert.AreEqual("password123", _user.Contraseña);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void User_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        _user.Id = Guid.Empty;
    }

    [TestMethod]
    public void User_ShouldSetValidId()
    {
        var validId = Guid.NewGuid();
        _user.Id = validId;
        Assert.AreEqual(validId, _user.Id);
    }

    [TestMethod]
    public void User_ShouldSetValidFechaNacimiento()
    {
        var validDate = DateTime.Now.AddYears(-25);
        _user.FechaNacimiento = validDate;
        Assert.AreEqual(validDate, _user.FechaNacimiento);
    }

    [TestMethod]
    public void User_ShouldAllowMultipleRoles()
    {
        _user.Roles = [new RolVisitante(), new RolOperadorAtraccion()];

        Assert.AreEqual(2, _user.Roles.Count);
        Assert.IsTrue(_user.Roles.Any(r => r is RolVisitante));
        Assert.IsTrue(_user.Roles.Any(r => r is RolOperadorAtraccion));
    }

    [TestMethod]
    public void User_ShouldInitializeWithEmptyCodigoIdentificacion()
    {
        Assert.AreEqual(string.Empty, _user.CodigoIdentificacion);
    }

    [TestMethod]
    public void ObtenerRol_ShouldReturnRolVisitante_WhenUserHasRolVisitante()
    {
        _user.Roles = [new RolVisitante()];

        var rolVisitante = _user.ObtenerRol<RolVisitante>();

        Assert.IsNotNull(rolVisitante);
        Assert.IsInstanceOfType(rolVisitante, typeof(RolVisitante));
    }

    [TestMethod]
    public void TieneRol_ShouldReturnTrue_WhenUserHasRolVisitante()
    {
        _user.Roles = [new RolVisitante()];

        var tieneRol = _user.TieneRol<RolVisitante>();

        Assert.IsTrue(tieneRol);
    }

    [TestMethod]
    public void GetMembresia_ShouldReturnNivelMembresia_FromRolVisitante()
    {
        var rolVisitante = new RolVisitante { NivelMembresia = NivelMembresia.Premium };
        _user.Roles = [rolVisitante];

        var membresia = _user.GetMembresia();

        Assert.AreEqual(NivelMembresia.Premium, membresia);
    }
}
