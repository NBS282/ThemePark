using ThemePark.Entities.Tickets;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.DataAccess.Repositories;

public class TicketsRepository(ThemeParkDbContext context) : ITicketsRepository
{
    private readonly ThemeParkDbContext _context = context;

    public Ticket Create(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        _context.SaveChanges();
        return ticket;
    }

    public Ticket GetById(Guid id)
    {
        var ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
        if(ticket == null)
        {
            throw new TicketNotFoundException(id);
        }

        return ticket;
    }

    public Ticket GetByQRCode(Guid codigoQR)
    {
        var ticket = _context.Tickets.FirstOrDefault(t => t.CodigoQR == codigoQR);
        if(ticket == null)
        {
            throw new TicketNotFoundException(codigoQR.ToString());
        }

        return ticket;
    }

    public List<Ticket> GetByCodigoIdentificacionUsuario(string codigoIdentificacionUsuario)
    {
        return _context.Tickets.Where(t => t.CodigoIdentificacionUsuario == codigoIdentificacionUsuario).ToList();
    }

    public List<Ticket> GetAll()
    {
        return _context.Tickets.ToList();
    }

    public int CountTicketsByEventId(Guid eventId)
    {
        return _context.Tickets.OfType<EventTicket>().Count(t => t.EventoId == eventId);
    }
}
