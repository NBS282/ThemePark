using ThemePark.Entities.Tickets;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain.TicketsTests;

[TestClass]
public class TicketTests
{
    [TestMethod]
    public void Ticket_ShouldBeAbstractClass()
    {
        var ticketType = typeof(Ticket);

        Assert.IsTrue(ticketType.IsAbstract);
    }

    [TestMethod]
    public void Ticket_ShouldHaveIdentificationProperties()
    {
        var ticketType = typeof(Ticket);

        Assert.IsNotNull(ticketType.GetProperty("Id"));
        Assert.IsNotNull(ticketType.GetProperty("CodigoQR"));
        Assert.IsNotNull(ticketType.GetProperty("CodigoIdentificacionUsuario"));
    }

    [TestMethod]
    public void Ticket_ShouldHaveDateProperties()
    {
        var ticketType = typeof(Ticket);

        Assert.IsNotNull(ticketType.GetProperty("FechaVisita"));
        Assert.IsNotNull(ticketType.GetProperty("FechaCompra"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTicketException))]
    public void Ticket_ShouldThrowException_WhenCodigoIdentificacionUsuarioIsEmpty()
    {
        var ticket = new GeneralTicket();
        ticket.CodigoIdentificacionUsuario = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTicketException))]
    public void Ticket_ShouldThrowException_WhenFechaVisitaIsInPast()
    {
        var ticket = new GeneralTicket();
        ticket.FechaVisita = DateTime.Now.AddDays(-1);
    }

    [TestMethod]
    public void Ticket_ShouldSetValidCodigoIdentificacionUsuario()
    {
        var ticket = new GeneralTicket();
        var validCodigo = "USER123";

        ticket.CodigoIdentificacionUsuario = validCodigo;

        Assert.AreEqual(validCodigo, ticket.CodigoIdentificacionUsuario);
    }

    [TestMethod]
    public void Ticket_ShouldSetValidFechaVisita()
    {
        var ticket = new GeneralTicket();
        var futureDate = DateTime.Now.AddDays(1);

        ticket.FechaVisita = futureDate;

        Assert.AreEqual(futureDate, ticket.FechaVisita);
    }

    [TestMethod]
    public void Ticket_ShouldSetAndGetAllProperties()
    {
        var ticket = new GeneralTicket();
        var id = Guid.NewGuid();
        var codigoQR = Guid.NewGuid();
        var fechaCompra = DateTime.Now;

        ticket.Id = id;
        ticket.CodigoQR = codigoQR;
        ticket.FechaCompra = fechaCompra;

        Assert.AreEqual(id, ticket.Id);
        Assert.AreEqual(codigoQR, ticket.CodigoQR);
        Assert.AreEqual(fechaCompra, ticket.FechaCompra);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTicketException))]
    public void Ticket_ShouldThrowException_WhenCodigoIdentificacionUsuarioIsNull()
    {
        var ticket = new GeneralTicket();
        ticket.CodigoIdentificacionUsuario = null!;
    }
}
