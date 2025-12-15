using ThemePark.Entities.Tickets;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain.TicketsTests;

[TestClass]
public class EventTicketTests
{
    [TestMethod]
    public void EventTicket_ShouldInheritFromTicket()
    {
        var ticket = new EventTicket();
        Assert.IsInstanceOfType(ticket, typeof(Ticket));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTicketException))]
    public void EventTicket_ShouldThrowException_WhenEventoIdIsEmpty()
    {
        var ticket = new EventTicket();
        ticket.EventoId = Guid.Empty;
    }

    [TestMethod]
    public void EventTicket_ShouldSetValidEventoId()
    {
        var ticket = new EventTicket();
        var validEventoId = Guid.NewGuid();

        ticket.EventoId = validEventoId;

        Assert.AreEqual(validEventoId, ticket.EventoId);
    }
}
