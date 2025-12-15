using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.BusinessLogic.Attractions;

public class AttractionManagementService(
    IAttractionRepository attractionRepository,
    IDateTimeRepository dateTimeRepository,
    IMaintenanceRepository maintenanceRepository,
    IIncidentRepository incidentRepository) : IAttractionManagementService
{
    private readonly IAttractionRepository _attractionRepository = attractionRepository;
    private readonly IDateTimeRepository _dateTimeRepository = dateTimeRepository;
    private readonly IMaintenanceRepository _maintenanceRepository = maintenanceRepository;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;

    public Attraction CreateAttraction(string nombre, int tipo, int edadMinima, int capacidadMaxima, string descripcion, int points)
    {
        if(_attractionRepository.ExistsByName(nombre))
        {
            throw BusinessLogicException.AttractionAlreadyExists(nombre);
        }

        if(!Enum.IsDefined(typeof(TipoAtraccion), tipo))
        {
            throw BusinessLogicException.InvalidAttractionType(tipo.ToString());
        }

        var tipoAtraccion = (TipoAtraccion)tipo;
        var fechaCreacion = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var attraction = new Attraction(nombre, tipoAtraccion, edadMinima, capacidadMaxima, descripcion, fechaCreacion, points);

        _attractionRepository.Save(attraction);

        return attraction;
    }

    public void DeleteAttraction(string nombre)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        var activeIncidents = _incidentRepository.GetActiveByAttractionName(nombre);
        if(activeIncidents.Any())
        {
            throw new CannotDeleteAttractionWithActiveIncidentsException(nombre);
        }

        var scheduledMaintenances = _maintenanceRepository.GetByAttractionName(nombre);
        if(scheduledMaintenances.Any())
        {
            throw new CannotDeleteAttractionWithScheduledMaintenancesException(nombre);
        }

        _attractionRepository.Delete(nombre);
    }

    public Attraction UpdateAttraction(string nombre, string descripcion, int? capacidadMaxima, int? edadMinima)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        var attraction = _attractionRepository.GetByName(nombre);
        var fechaModificacion = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;

        attraction.UpdateInfo(descripcion, capacidadMaxima, edadMinima, fechaModificacion);
        _attractionRepository.Save(attraction);

        return attraction;
    }

    public Attraction GetCapacity(string nombre)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        CheckAndActivateScheduledMaintenances(nombre);

        var attraction = _attractionRepository.GetByName(nombre);

        return attraction;
    }

    private void CheckAndActivateScheduledMaintenances(string attractionName)
    {
        var currentDateTime = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var maintenances = _maintenanceRepository.GetByAttractionName(attractionName);

        foreach(var maintenance in maintenances)
        {
            var scheduledDateTime = maintenance.Fecha.Add(maintenance.HoraInicio);

            if(currentDateTime >= scheduledDateTime)
            {
                var incident = _incidentRepository.GetById(maintenance.IncidentId);

                if(incident != null && incident.IsActive && incident.IsMaintenanceIncident())
                {
                    var attraction = _attractionRepository.GetByName(attractionName);

                    if(!attraction.TieneIncidenciaActiva)
                    {
                        attraction.ActivarIncidencia();
                        _attractionRepository.Save(attraction);
                    }
                }
            }
        }
    }

    public List<Attraction> GetAll()
    {
        var attractions = _attractionRepository.GetAll();

        foreach(var attraction in attractions)
        {
            CheckAndActivateScheduledMaintenances(attraction.Nombre);
        }

        var updatedAttractions = new List<Attraction>();
        foreach(var attraction in attractions)
        {
            var updated = _attractionRepository.GetByName(attraction.Nombre);
            updatedAttractions.Add(updated);
        }

        return updatedAttractions;
    }

    public Attraction GetById(string nombre)
    {
        var attraction = _attractionRepository.GetById(nombre);
        if(attraction == null)
        {
            throw new AttractionNotFoundException(nombre);
        }

        return attraction;
    }
}
