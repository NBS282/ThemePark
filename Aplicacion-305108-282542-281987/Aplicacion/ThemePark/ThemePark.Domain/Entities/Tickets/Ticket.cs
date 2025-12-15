using ThemePark.Exceptions;
namespace ThemePark.Entities.Tickets;

public abstract class Ticket
{
    public Guid Id { get; set; }
    public Guid CodigoQR { get; set; }
    private DateTime _fechaVisita;
    public DateTime FechaVisita
    {
        get => _fechaVisita;
        set
        {
            if(value.Date < DateTime.Now.Date)
            {
                throw new InvalidTicketException("fecha de visita está en el pasado");
            }

            _fechaVisita = value;
        }
    }

    private string _codigoIdentificacionUsuario = string.Empty;
    public string CodigoIdentificacionUsuario
    {
        get => _codigoIdentificacionUsuario;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidTicketException("código de identificación de usuario está vacío");
            }

            _codigoIdentificacionUsuario = value;
        }
    }

    public DateTime FechaCompra { get; set; }
}
