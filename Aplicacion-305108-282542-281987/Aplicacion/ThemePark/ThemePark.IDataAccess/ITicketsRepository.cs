using ThemePark.Entities.Tickets;

namespace ThemePark.IDataAccess;

public interface ITicketsRepository
{
    Ticket Create(Ticket ticket);
    Ticket GetById(Guid id);
    Ticket GetByQRCode(Guid codigoQR);
    List<Ticket> GetByCodigoIdentificacionUsuario(string codigoIdentificacionUsuario);
    List<Ticket> GetAll();
    int CountTicketsByEventId(Guid eventId);
}
