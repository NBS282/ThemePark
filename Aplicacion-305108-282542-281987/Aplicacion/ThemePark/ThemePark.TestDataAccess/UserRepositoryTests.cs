using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.TestDataAccess;

[TestClass]
public class UserRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private UserRepository _repository = null!;
    private User _testUser = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new UserRepository(_context);

        _testUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Now
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
        _context = null!;
        _repository = null!;
        _testUser = null!;
    }

    [TestMethod]
    public void Add_ShouldReturnUser_WhenValidUserProvided()
    {
        var result = _repository.Add(_testUser);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        Assert.AreEqual(_testUser.Email, result.Email);
    }

    [TestMethod]
    public void Add_ShouldReturnUserWithCorrectNombre_WhenValidUserProvided()
    {
        var result = _repository.Add(_testUser);

        Assert.AreEqual(_testUser.Nombre, result.Nombre);
    }

    [TestMethod]
    public void ExistsByEmail_ShouldReturnTrue_WhenEmailExists()
    {
        _repository.Add(_testUser);

        var result = _repository.ExistsByEmail("juan@email.com");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllUsers_WhenUsersExist()
    {
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "García",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = DateTime.Parse("1988-03-20"),
            Roles = [new RolAdministradorParque()],
            FechaRegistro = DateTime.Now
        };

        _repository.Add(_testUser);
        _repository.Add(user2);

        var result = _repository.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(u => u.Email == "juan@email.com"));
        Assert.IsTrue(result.Any(u => u.Email == "maria@email.com"));
    }

    [TestMethod]
    public void GetById_ShouldReturnUser_WhenUserExists()
    {
        _repository.Add(_testUser);

        var result = _repository.GetById(_testUser.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        Assert.AreEqual(_testUser.Email, result.Email);
    }

    [TestMethod]
    public void GetById_ShouldReturnUserWithCorrectNombre_WhenUserExists()
    {
        _repository.Add(_testUser);

        var result = _repository.GetById(_testUser.Id);

        Assert.AreEqual(_testUser.Nombre, result.Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(UserNotFoundException))]
    public void GetById_ShouldThrowArgumentException_WhenUserNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetById(nonExistentId);
    }

    [TestMethod]
    public void Update_ShouldReturnUpdatedUser_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var updatedUser = new User
        {
            Id = _testUser.Id,
            Nombre = "Juan Carlos",
            Apellido = "Pérez González",
            Email = "nuevo@email.com",
            Contraseña = "nuevapassword123",
            FechaNacimiento = DateTime.Parse("1985-03-10"),
            Roles = [new RolAdministradorParque()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Update(updatedUser);

        Assert.IsNotNull(result);
        Assert.AreEqual(updatedUser.Id, result.Id);
        Assert.AreEqual("Juan Carlos", result.Nombre);
    }

    [TestMethod]
    public void Update_ShouldUpdateUserPersonalInfo_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var updatedUser = new User
        {
            Id = _testUser.Id,
            Nombre = "Juan Carlos",
            Apellido = "Pérez González",
            Email = "nuevo@email.com",
            Contraseña = "nuevapassword123",
            FechaNacimiento = DateTime.Parse("1985-03-10"),
            Roles = [new RolAdministradorParque()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Update(updatedUser);

        Assert.AreEqual("Pérez González", result.Apellido);
        Assert.AreEqual("nuevo@email.com", result.Email);
        Assert.AreEqual("nuevapassword123", result.Contraseña);
    }

    [TestMethod]
    public void Update_ShouldUpdateUserBirthDate_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var updatedUser = new User
        {
            Id = _testUser.Id,
            Nombre = "Juan Carlos",
            Apellido = "Pérez González",
            Email = "nuevo@email.com",
            Contraseña = "nuevapassword123",
            FechaNacimiento = DateTime.Parse("1985-03-10"),
            Roles = [new RolAdministradorParque()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Update(updatedUser);

        Assert.AreEqual(DateTime.Parse("1985-03-10"), result.FechaNacimiento);
    }

    [TestMethod]
    public void Update_ShouldPreserveOriginalRolesAndRegistration_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var updatedUser = new User
        {
            Id = _testUser.Id,
            Nombre = "Juan Carlos",
            Apellido = "Pérez González",
            Email = "nuevo@email.com",
            Contraseña = "nuevapassword123",
            FechaNacimiento = DateTime.Parse("1985-03-10"),
            Roles = [new RolAdministradorParque()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Update(updatedUser);

        Assert.AreEqual(_testUser.GetMembresia(), result.GetMembresia());
        Assert.IsTrue(result.Roles.Any(r => r is RolVisitante));
        Assert.AreEqual(_testUser.FechaRegistro.Date, result.FechaRegistro.Date);
    }

    [TestMethod]
    public void Add_ShouldPersistMultipleRoles_WhenUserHasMultipleRoles()
    {
        var userWithMultipleRoles = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Admin",
            Apellido = "Operador",
            Email = "admin.operador@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1980-01-01"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Premium }, new RolAdministradorParque(), new RolOperadorAtraccion()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(userWithMultipleRoles);

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Roles.Count);
        Assert.IsTrue(result.Roles.Any(r => r is RolVisitante));
    }

    [TestMethod]
    public void Add_ShouldPersistAllRoleTypes_WhenUserHasMultipleRoles()
    {
        var userWithMultipleRoles = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Admin",
            Apellido = "Operador",
            Email = "admin.operador@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1980-01-01"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Premium }, new RolAdministradorParque(), new RolOperadorAtraccion()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(userWithMultipleRoles);

        Assert.IsTrue(result.Roles.Any(r => r is RolAdministradorParque));
        Assert.IsTrue(result.Roles.Any(r => r is RolOperadorAtraccion));
    }

    [TestMethod]
    public void UpdateAdmin_ShouldUpdateRolesAndMembership_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var adminUpdate = new User
        {
            Id = _testUser.Id,
            Roles = [new RolAdministradorParque(), new RolOperadorAtraccion()]
        };

        var result = _repository.UpdateAdmin(adminUpdate);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        Assert.AreEqual(2, result.Roles.Count);
    }

    [TestMethod]
    public void UpdateAdmin_ShouldUpdateRoles_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var adminUpdate = new User
        {
            Id = _testUser.Id,
            Roles = [new RolAdministradorParque(), new RolOperadorAtraccion()]
        };

        var result = _repository.UpdateAdmin(adminUpdate);

        Assert.IsTrue(result.Roles.Any(r => r is RolAdministradorParque));
        Assert.IsTrue(result.Roles.Any(r => r is RolOperadorAtraccion));
    }

    [TestMethod]
    public void UpdateAdmin_ShouldPreserveOtherUserFields_WhenValidUserProvided()
    {
        _repository.Add(_testUser);

        var adminUpdate = new User
        {
            Id = _testUser.Id,
            Roles = [new RolAdministradorParque(), new RolOperadorAtraccion()]
        };

        var result = _repository.UpdateAdmin(adminUpdate);

        Assert.AreEqual(_testUser.Nombre, result.Nombre);
        Assert.AreEqual(_testUser.Email, result.Email);
        Assert.AreEqual(_testUser.FechaRegistro.Date, result.FechaRegistro.Date);
    }

    [TestMethod]
    public void ExistsByEmailExcludingUser_ShouldReturnFalse_WhenEmailBelongsToSameUser()
    {
        _repository.Add(_testUser);

        var result = _repository.ExistsByEmailExcludingUser(_testUser.Email, _testUser.Id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetByEmailAndPassword_ShouldReturnUser_WhenCredentialsAreValid()
    {
        _repository.Add(_testUser);

        var result = _repository.GetByEmailAndPassword("juan@email.com", "password123");

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        Assert.AreEqual(_testUser.Email, result.Email);
    }

    [TestMethod]
    public void GetByEmailAndPassword_ShouldReturnUserWithCorrectNombre_WhenCredentialsAreValid()
    {
        _repository.Add(_testUser);

        var result = _repository.GetByEmailAndPassword("juan@email.com", "password123");

        Assert.AreEqual(_testUser.Nombre, result.Nombre);
    }

    [TestMethod]
    public void GetByEmailAndPassword_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        _repository.Add(_testUser);

        var result = _repository.GetByEmailAndPassword("juan@email.com", "wrongpassword");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByEmailAndPassword_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        _repository.Add(_testUser);

        var result = _repository.GetByEmailAndPassword("noexiste@email.com", "password123");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByCodigoIdentificacion_ShouldReturnUser_WhenCodigoExists()
    {
        _testUser.CodigoIdentificacion = "12345678";
        _repository.Add(_testUser);

        var result = _repository.GetByCodigoIdentificacion("12345678");

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        Assert.AreEqual(_testUser.CodigoIdentificacion, result.CodigoIdentificacion);
    }

    [TestMethod]
    public void GetByCodigoIdentificacion_ShouldReturnUserWithCorrectEmail_WhenCodigoExists()
    {
        _testUser.CodigoIdentificacion = "12345678";
        _repository.Add(_testUser);

        var result = _repository.GetByCodigoIdentificacion("12345678");

        Assert.AreEqual(_testUser.Email, result.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(UserNotFoundException))]
    public void GetByCodigoIdentificacion_ShouldThrowArgumentException_WhenCodigoNotFound()
    {
        _testUser.CodigoIdentificacion = "12345678";
        _repository.Add(_testUser);

        _repository.GetByCodigoIdentificacion("87654321");
    }

    [TestMethod]
    public void Add_ShouldAssignAdminRole_WhenFirstUserHasNoRoles()
    {
        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Primer",
            Apellido = "Usuario",
            Email = "primero@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = null!,
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(firstUser);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Roles);
        Assert.AreEqual(2, result.Roles.Count);
    }

    [TestMethod]
    public void Add_ShouldAssignBothRoles_WhenFirstUserHasNoRoles()
    {
        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Primer",
            Apellido = "Usuario",
            Email = "primero@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = null!,
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(firstUser);

        Assert.IsTrue(result.Roles.Any(r => r is RolAdministradorParque));
        Assert.IsTrue(result.Roles.Any(r => r is RolVisitante));
    }

    [TestMethod]
    public void Add_ShouldAssignAdminRole_WhenFirstUserHasEmptyRoles()
    {
        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Primer",
            Apellido = "Usuario",
            Email = "primero@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = [],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(firstUser);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Roles);
        Assert.AreEqual(2, result.Roles.Count);
    }

    [TestMethod]
    public void Add_ShouldAssignBothRoles_WhenFirstUserHasEmptyRoles()
    {
        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Primer",
            Apellido = "Usuario",
            Email = "primero@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = [],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(firstUser);

        Assert.IsTrue(result.Roles.Any(r => r is RolAdministradorParque));
        Assert.IsTrue(result.Roles.Any(r => r is RolVisitante));
    }

    [TestMethod]
    public void Add_ShouldNotAssignAdminRole_WhenFirstUserHasNonVisitanteRole()
    {
        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Primer",
            Apellido = "Usuario",
            Email = "primero@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = [new RolOperadorAtraccion()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(firstUser);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Roles);
        Assert.AreEqual(1, result.Roles.Count);
    }

    [TestMethod]
    public void Add_ShouldMaintainOriginalRole_WhenFirstUserHasNonVisitanteRole()
    {
        var firstUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Primer",
            Apellido = "Usuario",
            Email = "primero@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = [new RolOperadorAtraccion()],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(firstUser);

        Assert.IsInstanceOfType(result.Roles[0], typeof(RolOperadorAtraccion));
    }

    [TestMethod]
    public void Add_ShouldNotAssignAdminRole_WhenUsersAlreadyExist()
    {
        _repository.Add(_testUser);
        var secondUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Segundo",
            Apellido = "Usuario",
            Email = "segundo@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = [],
            FechaRegistro = DateTime.Now
        };

        var result = _repository.Add(secondUser);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Roles);
        Assert.AreEqual(1, result.Roles.Count);
        Assert.IsInstanceOfType(result.Roles[0], typeof(RolVisitante));
        Assert.IsFalse(result.Roles.Any(r => r is RolAdministradorParque));
    }

    [TestMethod]
    public void Delete_ShouldRemoveUser_WhenUserExists()
    {
        _repository.Add(_testUser);

        _repository.Delete(_testUser.Id);

        var allUsers = _repository.GetAll();
        Assert.AreEqual(0, allUsers.Count);
    }

    [TestMethod]
    public void Exists_ShouldReturnTrue_WhenUserExists()
    {
        _repository.Add(_testUser);

        var result = _repository.Exists(_testUser.Id);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsByCodigoIdentificacion_ShouldReturnTrue_WhenCodigoExists()
    {
        _testUser.CodigoIdentificacion = "TEST123";
        _repository.Add(_testUser);

        var result = _repository.ExistsByCodigoIdentificacion("TEST123");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsByCodigoIdentificacion_ShouldReturnFalse_WhenCodigoNotExists()
    {
        _testUser.CodigoIdentificacion = "TEST123";
        _repository.Add(_testUser);

        var result = _repository.ExistsByCodigoIdentificacion("DIFFERENT");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TPH_ShouldPersistNivelMembresia_WhenUserHasRolVisitante()
    {
        _repository.Add(_testUser);

        var userWithVIP = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "VIP",
            Apellido = "User",
            Email = "vip@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-01"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.VIP }],
            FechaRegistro = DateTime.Now
        };

        var savedUser = _repository.Add(userWithVIP);
        var retrievedUser = _repository.GetById(savedUser.Id);

        Assert.AreEqual(NivelMembresia.VIP, retrievedUser.GetMembresia());
        var rolVisitante = retrievedUser.ObtenerRol<RolVisitante>();
        Assert.IsNotNull(rolVisitante);
        Assert.AreEqual(NivelMembresia.VIP, rolVisitante.NivelMembresia);
    }
}
