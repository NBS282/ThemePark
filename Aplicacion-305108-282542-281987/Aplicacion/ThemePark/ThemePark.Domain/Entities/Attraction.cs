using ThemePark.Enums;
using ThemePark.Exceptions;
namespace ThemePark.Entities;

public class Attraction
{
    public string Nombre { get; private set; }
    public TipoAtraccion Tipo { get; private set; }
    public int EdadMinima { get; private set; }
    public int CapacidadMaxima { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaModificacion { get; private set; }
    public int AforoActual { get; private set; }
    public bool TieneIncidenciaActiva { get; private set; }
    public int Points { get; private set; }

    public Attraction(string nombre, TipoAtraccion tipo, int edadMinima, int capacidadMaxima, string descripcion, DateTime fechaCreacion, int points = 0)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new InvalidAttractionDataException("Nombre");
        }

        if(edadMinima < 0)
        {
            throw new InvalidAttractionDataException("EdadMinima", edadMinima.ToString());
        }

        if(capacidadMaxima <= 0)
        {
            throw new InvalidAttractionDataException("CapacidadMaxima", capacidadMaxima.ToString());
        }

        Nombre = nombre;
        Tipo = tipo;
        EdadMinima = edadMinima;
        CapacidadMaxima = capacidadMaxima;
        Descripcion = descripcion;
        FechaCreacion = fechaCreacion;
        FechaModificacion = null;
        AforoActual = 0;
        TieneIncidenciaActiva = false;
        Points = points;
    }

    public void IncrementarAforo()
    {
        if(AforoActual >= CapacidadMaxima)
        {
            throw new CapacityExceededException(Nombre, AforoActual, CapacidadMaxima);
        }

        AforoActual++;
    }

    public void DecrementarAforo()
    {
        if(AforoActual <= 0)
        {
            throw new InvalidAttractionDataException("No se puede decrementar el aforo cuando está en cero", "Cannot decrement capacity when it is at zero");
        }

        AforoActual--;
    }

    public void ActivarIncidencia()
    {
        TieneIncidenciaActiva = true;
    }

    public void DesactivarIncidencia()
    {
        TieneIncidenciaActiva = false;
    }

    public void UpdateInfo(string? descripcion = null, int? capacidadMaxima = null, int? edadMinima = null, DateTime? fechaModificacion = null)
    {
        if(!string.IsNullOrWhiteSpace(descripcion))
        {
            Descripcion = descripcion;
        }

        if(capacidadMaxima.HasValue)
        {
            if(capacidadMaxima.Value <= 0)
            {
                throw new InvalidAttractionDataException("CapacidadMaxima", capacidadMaxima.Value.ToString());
            }

            CapacidadMaxima = capacidadMaxima.Value;
        }

        if(edadMinima.HasValue)
        {
            if(edadMinima.Value < 0)
            {
                throw new InvalidAttractionDataException("EdadMinima", edadMinima.Value.ToString());
            }

            EdadMinima = edadMinima.Value;
        }

        FechaModificacion = fechaModificacion ?? DateTime.Now;
    }
}
