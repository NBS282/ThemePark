using ThemePark.Enums;
using ThemePark.Exceptions;

namespace ThemePark.Entities;

public class Reward
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Descripcion { get; private set; }
    public int CostoPuntos { get; private set; }
    public int CantidadDisponible { get; private set; }
    public NivelMembresia? NivelMembresiaRequerido { get; private set; }
    public bool Activa { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    public Reward(string nombre, string descripcion, int costoPuntos, int cantidadDisponible, NivelMembresia? nivelMembresiaRequerido)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new InvalidRequestDataException("El nombre de la recompensa no puede estar vacío");
        }

        if(string.IsNullOrWhiteSpace(descripcion))
        {
            throw new InvalidRequestDataException("La descripción de la recompensa no puede estar vacía");
        }

        if(costoPuntos <= 0)
        {
            throw new InvalidRequestDataException("El costo en puntos debe ser mayor a 0");
        }

        if(cantidadDisponible < 0)
        {
            throw new InvalidRequestDataException("La cantidad disponible no puede ser negativa");
        }

        Nombre = nombre;
        Descripcion = descripcion;
        CostoPuntos = costoPuntos;
        CantidadDisponible = cantidadDisponible;
        NivelMembresiaRequerido = nivelMembresiaRequerido;
        Activa = true;
        FechaCreacion = DateTime.Now;
    }

    public void DecrementarStock()
    {
        if(CantidadDisponible == 0)
        {
            throw new InvalidRequestDataException("No hay stock disponible de esta recompensa");
        }

        CantidadDisponible--;
    }

    public void UpdateInfo(string descripcion, int costoPuntos, int cantidadDisponible, NivelMembresia? nivelMembresiaRequerido)
    {
        if(string.IsNullOrWhiteSpace(descripcion))
        {
            throw new InvalidRequestDataException("La descripción de la recompensa no puede estar vacía");
        }

        if(costoPuntos <= 0)
        {
            throw new InvalidRequestDataException("El costo en puntos debe ser mayor a 0");
        }

        if(cantidadDisponible < 0)
        {
            throw new InvalidRequestDataException("La cantidad disponible no puede ser negativa");
        }

        Descripcion = descripcion;
        CostoPuntos = costoPuntos;
        CantidadDisponible = cantidadDisponible;
        NivelMembresiaRequerido = nivelMembresiaRequerido;
    }

    public void Desactivar()
    {
        Activa = false;
    }
}
