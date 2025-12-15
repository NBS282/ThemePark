using ThemePark.Exceptions;
namespace ThemePark.Entities;

public class Event
{
    private Guid _id;
    private string _name = string.Empty;

    public Guid Id
    {
        get => _id;
        set
        {
            if(value == Guid.Empty)
            {
                throw new InvalidEventDataException("Id", "vacío");
            }

            _id = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidEventDataException("Name");
            }

            _name = value;
        }
    }

    private DateTime _fecha;

    public DateTime Fecha
    {
        get => _fecha;
        set
        {
            if(value < DateTime.Now.Date)
            {
                throw new InvalidEventDataException("Fecha", "en el pasado");
            }

            _fecha = value;
        }
    }

    public TimeSpan Hora { get; set; }

    public int Aforo { get; set; }

    public decimal CostoAdicional { get; set; }

    public List<Attraction> Atracciones { get; set; } = [];
}
