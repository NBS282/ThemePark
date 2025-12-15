using ThemePark.Entities.Tickets;

namespace ThemePark.TestDomain.TicketsTests;

[TestClass]
public class GeneralTicketTests
{
    [TestMethod]
    public void GeneralTicket_ShouldInheritFromTicket()
    {
        var ticket = new GeneralTicket();
        Assert.IsInstanceOfType(ticket, typeof(Ticket));
    }
}
