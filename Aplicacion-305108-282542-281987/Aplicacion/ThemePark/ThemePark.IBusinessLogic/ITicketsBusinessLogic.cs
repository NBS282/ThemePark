using ThemePark.Entities.Tickets;

namespace ThemePark.IBusinessLogic;

public interface ITicketsBusinessLogic
{
    Ticket CreateTicket(Ticket ticket);
    Ticket GetTicketById(Guid id);
    List<Ticket> GetAllTickets();
    List<Ticket> GetMyTickets();
}
