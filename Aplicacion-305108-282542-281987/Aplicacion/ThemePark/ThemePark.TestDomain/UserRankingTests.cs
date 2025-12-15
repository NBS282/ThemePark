using ThemePark.Entities;

namespace ThemePark.TestDomain;

[TestClass]
public class UserRankingTests
{
    [TestMethod]
    public void Constructor_ShouldSetRankingProperties_WhenValidParametersProvided()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new ThemePark.Entities.Roles.RolVisitante()],
            FechaRegistro = DateTime.Now
        };
        var fecha = DateTime.Today;

        var userRanking = new UserRanking(1, user, 100, fecha);

        Assert.AreEqual(1, userRanking.Posicion);
        Assert.AreEqual(100, userRanking.Puntos);
        Assert.AreEqual(fecha, userRanking.Fecha);
    }

    [TestMethod]
    public void Constructor_ShouldSetUserRelation_WhenValidParametersProvided()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new ThemePark.Entities.Roles.RolVisitante()],
            FechaRegistro = DateTime.Now
        };
        var fecha = DateTime.Today;

        var userRanking = new UserRanking(1, user, 100, fecha);

        Assert.AreEqual(user, userRanking.Usuario);
    }

    [TestMethod]
    public void DefaultConstructor_ShouldCreateUserRankingWithDefaultNumericValues()
    {
        var userRanking = new UserRanking();

        Assert.AreEqual(0, userRanking.Posicion);
        Assert.AreEqual(0, userRanking.Puntos);
    }

    [TestMethod]
    public void DefaultConstructor_ShouldCreateUserRankingWithNullUserAndDefaultDate()
    {
        var userRanking = new UserRanking();

        Assert.IsNull(userRanking.Usuario);
        Assert.AreEqual(default(DateTime), userRanking.Fecha);
    }
}
