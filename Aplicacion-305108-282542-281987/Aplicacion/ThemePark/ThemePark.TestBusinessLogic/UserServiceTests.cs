using Moq;
using ThemePark.BusinessLogic;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IVisitRepository> _mockVisitRepository = null!;
    private Mock<ISessionRepository> _mockSessionRepository = null!;
    private Mock<IRewardExchangeRepository> _mockRewardExchangeRepository = null!;
    private IUserService _userService = null!;
    private User _testUser = null!;
    private User _testUser2 = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockVisitRepository = new Mock<IVisitRepository>(MockBehavior.Strict);
        _mockSessionRepository = new Mock<ISessionRepository>(MockBehavior.Strict);
        _mockRewardExchangeRepository = new Mock<IRewardExchangeRepository>(MockBehavior.Strict);
        _userService = new UserService(_mockUserRepository.Object, _mockVisitRepository.Object, _mockSessionRepository.Object, _mockRewardExchangeRepository.Object);

        _testUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@example.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            FechaRegistro = DateTime.Now,
            CodigoIdentificacion = "ABC12345"
        };

        _testUser2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@example.com",
            Contraseña = "password456",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            FechaRegistro = DateTime.Now
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockUserRepository = null!;
        _userService = null!;
        _testUser = null!;
        _testUser2 = null!;
    }

    [TestMethod]
    public void UserService_Register_ShouldReturnUserWithValidId()
    {
        _mockUserRepository.Setup(r => r.ExistsByEmail(_testUser.Email)).Returns(false);
        _mockUserRepository.Setup(r => r.ExistsByCodigoIdentificacion(It.IsAny<string>())).Returns(false);
        _mockUserRepository.Setup(r => r.Add(_testUser)).Returns(_testUser);

        var result = _userService.Register(_testUser);

        Assert.AreEqual(_testUser.Id, result.Id);
        Assert.AreEqual(_testUser.Nombre, result.Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(UserRegistrationException))]
    public void UserService_Register_ShouldThrowUserRegistrationException_WhenEmailAlreadyExists()
    {
        _mockUserRepository.Setup(r => r.ExistsByEmail(_testUser.Email)).Returns(true);

        _userService.Register(_testUser);
    }

    [TestMethod]
    public void UserService_UpdateProfile_ShouldReturnUpdatedUser()
    {
        var updatedUser = new User
        {
            Id = _testUser.Id,
            Nombre = "Carlos",
            Apellido = "González",
            Email = _testUser.Email,
            Contraseña = _testUser.Contraseña,
            FechaNacimiento = _testUser.FechaNacimiento,
            FechaRegistro = _testUser.FechaRegistro
        };

        _mockUserRepository.Setup(r => r.GetById(updatedUser.Id)).Returns(updatedUser);
        _mockUserRepository.Setup(r => r.ExistsByEmailExcludingUser(updatedUser.Email, updatedUser.Id)).Returns(false);
        _mockUserRepository.Setup(r => r.Update(updatedUser)).Returns(updatedUser);

        var result = _userService.UpdateProfile(updatedUser);

        Assert.AreEqual(updatedUser.Nombre, result.Nombre);
        Assert.AreEqual(updatedUser.Apellido, result.Apellido);
    }

    [TestMethod]
    [ExpectedException(typeof(UserNotFoundException))]
    public void UserService_UpdateProfile_ShouldThrowUserNotFoundException_WhenUserNotExists()
    {
        var nonExistentUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "NoExiste",
            Apellido = "Usuario",
            Email = "noexiste@example.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            FechaRegistro = DateTime.Now
        };

        _mockUserRepository.Setup(r => r.GetById(nonExistentUser.Id))
                          .Throws(new UserNotFoundException(nonExistentUser.Id));

        _userService.UpdateProfile(nonExistentUser);
    }

    [TestMethod]
    public void UserService_GetAllUsers_ShouldReturnValidListOfUsers()
    {
        var userList = new List<User> { _testUser, _testUser2 };
        _mockUserRepository.Setup(r => r.GetAll()).Returns(userList);

        var result = _userService.GetAllUsers();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        _mockUserRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void UserService_GetAllUsers_ShouldReturnUsersWithCorrectIds()
    {
        var userList = new List<User> { _testUser, _testUser2 };
        _mockUserRepository.Setup(r => r.GetAll()).Returns(userList);

        var result = _userService.GetAllUsers();

        Assert.AreEqual(_testUser.Id, result[0].Id);
        Assert.AreEqual(_testUser2.Id, result[1].Id);
    }

    [TestMethod]
    public void UserService_GetUserById_ShouldReturnValidUser()
    {
        _mockUserRepository.Setup(r => r.GetById(_testUser.Id)).Returns(_testUser);

        var result = _userService.GetUserById(_testUser.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        _mockUserRepository.Verify(r => r.GetById(_testUser.Id), Times.Once);
    }

    [TestMethod]
    public void UserService_GetUserById_ShouldReturnUserWithCorrectNombre()
    {
        _mockUserRepository.Setup(r => r.GetById(_testUser.Id)).Returns(_testUser);

        var result = _userService.GetUserById(_testUser.Id);

        Assert.AreEqual(_testUser.Nombre, result.Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(UserNotFoundException))]
    public void UserService_GetUserById_ShouldThrowUserNotFoundException_WhenUserNotExists()
    {
        var nonExistentId = Guid.NewGuid();
        _mockUserRepository.Setup(r => r.GetById(nonExistentId))
                          .Throws(new UserNotFoundException(nonExistentId));

        _userService.GetUserById(nonExistentId);
    }

    [TestMethod]
    public void UpdateUserPrivileges_ShouldReturnValidUserWithCorrectId_WhenValidUser()
    {
        var privilegeUser = new User
        {
            Id = _testUser.Id,
            Roles = [new RolAdministradorParque(), new RolOperadorAtraccion()]
        };

        _mockUserRepository.Setup(r => r.GetById(_testUser.Id))
                          .Returns(_testUser);
        _mockUserRepository.Setup(r => r.GetFounderAdminId()).Returns((Guid?)null);
        _mockUserRepository.Setup(r => r.UpdateAdmin(It.IsAny<User>()))
                          .Returns(privilegeUser);

        var result = _userService.UpdateUserPrivileges(privilegeUser);

        Assert.IsNotNull(result);
        Assert.AreEqual(privilegeUser.Id, result.Id);
        Assert.AreEqual(2, result.Roles.Count);
    }

    [TestMethod]
    public void UpdateUserPrivileges_ShouldReturnUserWithCorrectRoles_WhenValidUser()
    {
        var privilegeUser = new User
        {
            Id = _testUser.Id,
            Roles = [new RolAdministradorParque(), new RolOperadorAtraccion()]
        };

        _mockUserRepository.Setup(r => r.GetById(_testUser.Id))
                          .Returns(_testUser);
        _mockUserRepository.Setup(r => r.GetFounderAdminId())
                          .Returns((Guid?)null);
        _mockUserRepository.Setup(r => r.UpdateAdmin(It.IsAny<User>()))
                          .Returns(privilegeUser);

        var result = _userService.UpdateUserPrivileges(privilegeUser);

        Assert.IsTrue(result.Roles.Any(r => r is RolAdministradorParque));
        Assert.IsTrue(result.Roles.Any(r => r is RolOperadorAtraccion));
    }

    [TestMethod]
    public void UpdateUserPrivileges_ShouldCallRepositoryMethods_WhenValidUser()
    {
        var privilegeUser = new User
        {
            Id = _testUser.Id,
            Roles = [new RolAdministradorParque(), new RolOperadorAtraccion()]
        };

        _mockUserRepository.Setup(r => r.GetById(_testUser.Id))
                          .Returns(_testUser);
        _mockUserRepository.Setup(r => r.GetFounderAdminId())
                          .Returns((Guid?)null);
        _mockUserRepository.Setup(r => r.UpdateAdmin(It.IsAny<User>()))
                          .Returns(privilegeUser);

        var result = _userService.UpdateUserPrivileges(privilegeUser);

        _mockUserRepository.Verify(r => r.GetById(_testUser.Id), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAdmin(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void UpdateProfile_ShouldThrowInvalidUserDataException_WhenEmailIsEmpty()
    {
        _ = new User
        {
            Id = _testUser.Id,
            Nombre = "Juan Updated",
            Apellido = "Pérez Updated",
            Email = string.Empty,
            Contraseña = _testUser.Contraseña,
            FechaNacimiento = _testUser.FechaNacimiento,
            FechaRegistro = _testUser.FechaRegistro
        };
    }

    [TestMethod]
    [ExpectedException(typeof(UserRegistrationException))]
    public void UpdateProfile_ShouldThrowUserRegistrationException_WhenEmailAlreadyInUseByAnotherUser()
    {
        var userWithDuplicateEmail = new User
        {
            Id = _testUser.Id,
            Nombre = "Juan Updated",
            Apellido = "Pérez Updated",
            Email = "maria@example.com",
            Contraseña = _testUser.Contraseña,
            FechaNacimiento = _testUser.FechaNacimiento,
            FechaRegistro = _testUser.FechaRegistro
        };

        _mockUserRepository.Setup(r => r.GetById(userWithDuplicateEmail.Id)).Returns(userWithDuplicateEmail);
        _mockUserRepository.Setup(r => r.ExistsByEmailExcludingUser(userWithDuplicateEmail.Email, userWithDuplicateEmail.Id)).Returns(true);

        _userService.UpdateProfile(userWithDuplicateEmail);
    }

    [TestMethod]
    public void Delete_ShouldCallRepositoryDelete_WhenValidId()
    {
        _mockUserRepository.Setup(r => r.GetById(_testUser.Id)).Returns(_testUser);
        _mockUserRepository.Setup(r => r.GetFounderAdminId()).Returns((Guid?)null);
        _mockUserRepository.Setup(r => r.Delete(_testUser.Id));

        _userService.Delete(_testUser.Id);

        _mockUserRepository.Verify(r => r.GetById(_testUser.Id), Times.Once);
        _mockUserRepository.Verify(r => r.Delete(_testUser.Id), Times.Once);
    }

    [TestMethod]
    public void Register_ShouldGenerateUniqueCodigoIdentificacion_WhenUserHasEmptyCode()
    {
        var userWithoutCode = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Test",
            Apellido = "User",
            Email = "test@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            CodigoIdentificacion = string.Empty
        };

        _mockUserRepository.Setup(r => r.ExistsByEmail("test@email.com")).Returns(false);
        _mockUserRepository.Setup(r => r.ExistsByCodigoIdentificacion(It.IsAny<string>())).Returns(false);
        _mockUserRepository.Setup(r => r.Add(It.IsAny<User>())).Returns((User u) => u);

        var result = _userService.Register(userWithoutCode);

        Assert.IsFalse(string.IsNullOrEmpty(result.CodigoIdentificacion));
        _mockUserRepository.Verify(r => r.ExistsByCodigoIdentificacion(It.IsAny<string>()), Times.AtLeastOnce);
        _mockUserRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void GetUserByCodigoIdentificacion_ShouldReturnValidUserWithCorrectId_WhenValidCodigoIdentificacion()
    {
        _mockUserRepository.Setup(r => r.GetByCodigoIdentificacion(_testUser.CodigoIdentificacion)).Returns(_testUser);

        var result = _userService.GetUserByCodigoIdentificacion(_testUser.CodigoIdentificacion);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testUser.Id, result.Id);
        _mockUserRepository.Verify(r => r.GetByCodigoIdentificacion(_testUser.CodigoIdentificacion), Times.Once);
    }

    [TestMethod]
    public void GetUserByCodigoIdentificacion_ShouldReturnUserWithCorrectProperties_WhenValidCodigoIdentificacion()
    {
        _mockUserRepository.Setup(r => r.GetByCodigoIdentificacion(_testUser.CodigoIdentificacion)).Returns(_testUser);

        var result = _userService.GetUserByCodigoIdentificacion(_testUser.CodigoIdentificacion);

        Assert.AreEqual(_testUser.Nombre, result.Nombre);
        Assert.AreEqual(_testUser.CodigoIdentificacion, result.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(CannotModifyFounderAdminException))]
    public void Delete_ShouldThrowCannotModifyFounderAdminException_WhenDeletingFounderAdmin()
    {
        var founderId = Guid.NewGuid();
        var founderAdmin = new User
        {
            Id = founderId,
            Nombre = "Admin",
            Apellido = "Fundador",
            Email = "admin@themepark.com",
            Contraseña = "admin123",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            FechaRegistro = new DateTime(2025, 1, 1),
            Roles = [new RolAdministradorParque(), new RolVisitante()]
        };

        _mockUserRepository.Setup(r => r.GetById(founderId)).Returns(founderAdmin);
        _mockUserRepository.Setup(r => r.GetFounderAdminId()).Returns(founderId);

        _userService.Delete(founderId);
    }

    [TestMethod]
    [ExpectedException(typeof(CannotModifyFounderAdminException))]
    public void UpdateUserPrivileges_ShouldThrowCannotModifyFounderAdminException_WhenRemovingAdminRoleFromFounder()
    {
        var founderId = Guid.NewGuid();
        var founderAdmin = new User
        {
            Id = founderId,
            Nombre = "Admin",
            Apellido = "Fundador",
            Email = "admin@themepark.com",
            Contraseña = "admin123",
            FechaNacimiento = DateTime.Now.AddYears(-30),
            FechaRegistro = new DateTime(2025, 1, 1),
            Roles = [new RolAdministradorParque(), new RolVisitante()]
        };

        var updatedUser = new User
        {
            Id = founderId,
            Roles = [new RolVisitante()]
        };

        _mockUserRepository.Setup(r => r.GetById(founderId)).Returns(founderAdmin);
        _mockUserRepository.Setup(r => r.GetFounderAdminId()).Returns(founderId);

        _userService.UpdateUserPrivileges(updatedUser);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void UpdateUserPrivileges_ShouldThrowBusinessLogicException_WhenInvalidRoleType()
    {
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            Contraseña = "pass123",
            FechaNacimiento = DateTime.Now.AddYears(-25),
            FechaRegistro = DateTime.Now,
            Roles = [new RolVisitante()]
        };

        var invalidRole = new InvalidRolMock();

        var updatedUser = new User
        {
            Id = userId,
            Roles = [invalidRole]
        };

        _mockUserRepository.Setup(r => r.GetById(userId)).Returns(existingUser);
        _mockUserRepository.Setup(r => r.GetFounderAdminId()).Returns((Guid?)null);

        _userService.UpdateUserPrivileges(updatedUser);
    }

    private sealed class InvalidRolMock : Entities.Roles.Rol
    {
        public override Enums.Rol TipoRol => (Enums.Rol)999;
    }

    [TestMethod]
    public void BusinessLogicException_InvalidMembershipType_ShouldInheritFromBaseCustomException()
    {
        var exception = BusinessLogicException.InvalidMembershipType("3");

        Assert.IsInstanceOfType(exception, typeof(BusinessLogicException));
        Assert.AreEqual("Tipo de nivelMembresia inválido: '3'. Los valores válidos son: 0=Estándar, 1=Premium, 2=VIP", exception.Message);
        Assert.IsTrue(exception.TechnicalDetails.Contains("Business logic error"));
    }

    [TestMethod]
    public void BusinessLogicException_InvalidDateRange_ShouldHaveCorrectMessageAndType()
    {
        var exception = BusinessLogicException.InvalidDateRange();

        Assert.IsInstanceOfType(exception, typeof(BusinessLogicException));
        Assert.AreEqual("La fecha de inicio debe ser menor o igual a la fecha de fin", exception.Message);
        Assert.IsNotNull(exception.TechnicalDetails);
    }

    [TestMethod]
    public void BusinessLogicException_UserNotExists_ShouldBeThrowable()
    {
        try
        {
            throw BusinessLogicException.UserNotExists();
        }
        catch(BusinessLogicException ex)
        {
            Assert.AreEqual("Usuario no existe", ex.Message);
            Assert.IsTrue(ex.TechnicalDetails.Contains("Business logic error"));
        }
    }

    [TestMethod]
    public void BusinessLogicException_EventNotExists_ShouldHaveCorrectMessage()
    {
        var exception = BusinessLogicException.EventNotExists();

        Assert.AreEqual("Evento no existe", exception.Message);
        Assert.IsInstanceOfType(exception, typeof(BusinessLogicException));
    }

    [TestMethod]
    public void GetUserPointHistory_ShouldReturnListOfVisitsAndExchanges()
    {
        var userId = Guid.NewGuid();
        var visits = new List<Visit>
        {
            new Visit(userId, "Montaña Rusa", DateTime.Now.AddHours(-2), 50),
            new Visit(userId, "Casa Embrujada", DateTime.Now.AddHours(-1), 30)
        };
        var exchanges = new List<RewardExchange>
        {
            new RewardExchange(1, userId, 20, 80)
        };

        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(visits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(exchanges);

        var result = _userService.GetUserPointHistory();

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void GetUserPointHistory_ShouldReturnItemsOrderedByDate()
    {
        var userId = Guid.NewGuid();
        var entryTime1 = DateTime.Now.AddHours(-3);
        var entryTime2 = DateTime.Now.AddHours(-1);
        var visits = new List<Visit>
        {
            new Visit(userId, "Montaña Rusa", entryTime1, 50),
            new Visit(userId, "Casa Embrujada", entryTime2, 30)
        };
        var exchanges = new List<RewardExchange>();

        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(visits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(exchanges);

        var result = _userService.GetUserPointHistory();

        Assert.AreEqual(2, result.Count);
        var firstItem = result[0] as Visit;
        var secondItem = result[1] as Visit;
        Assert.IsNotNull(firstItem);
        Assert.IsNotNull(secondItem);
        Assert.IsTrue(firstItem.EntryTime > secondItem.EntryTime);
    }

    [TestMethod]
    public void GetUserPoints_ShouldReturnObjectWithPointsCalculation()
    {
        var userId = Guid.NewGuid();
        var visits = new List<Visit>
        {
            new Visit(userId, "Montaña Rusa", DateTime.Now.AddHours(-3), 50),
            new Visit(userId, "Casa Embrujada", DateTime.Now.AddHours(-2), 30),
            new Visit(userId, "Rueda Gigante", DateTime.Now.AddHours(-1), 20)
        };

        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(visits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns([]);

        var result = _userService.GetUserPoints();

        Assert.IsNotNull(result);
        var resultType = result.GetType();
        var puntosAcumulados = (int)(resultType.GetProperty("PuntosAcumulados")?.GetValue(result) ?? 0);
        var puntosDisponibles = (int)(resultType.GetProperty("PuntosDisponibles")?.GetValue(result) ?? 0);
        var puntosGastados = (int)(resultType.GetProperty("PuntosGastados")?.GetValue(result) ?? 0);

        Assert.AreEqual(100, puntosAcumulados);
        Assert.AreEqual(100, puntosDisponibles);
        Assert.AreEqual(0, puntosGastados);
    }
}
