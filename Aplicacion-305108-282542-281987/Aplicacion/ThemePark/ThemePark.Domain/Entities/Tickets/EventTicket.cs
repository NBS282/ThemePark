using ThemePark.Exceptions;
namespace ThemePark.Entities.Tickets;

public class EventTicket : Ticket
{
    private Guid _eventoId;
    public Guid EventoId
    {
        get => _eventoId;
        set
        {
            if(value == Guid.Empty)
            {
                throw new InvalidTicketException("evento no puede estar vacío");
            }

            _eventoId = value;
        }
    }
}
