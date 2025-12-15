using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic.Algorithms;

public class PuntuacionPorEventoAlgorithm(IEventRepository eventRepository) : IScoringAlgorithm
{
    private readonly IEventRepository _eventRepository = eventRepository;

    public int CalculatePoints(Visit visit, Configuracion configuration, List<Visit> userVisits)
    {
        if(configuration is not ConfiguracionPorEvento eventoConfig)
        {
            throw ScoringStrategyException.InvalidConfigurationType("ConfiguracionPorEvento");
        }

        var events = _eventRepository.Get(e => e.Name == eventoConfig.Evento);
        var activeEvent = events.FirstOrDefault();

        var basePoints = visit.Attraction?.Points ?? 0;

        if(activeEvent != null && visit.Attraction != null)
        {
            var eventStart = activeEvent.Fecha.Date + activeEvent.Hora;
            var eventEnd = eventStart.AddHours(4);

            var isDuringEvent = visit.EntryTime >= eventStart && visit.EntryTime <= eventEnd;
            var isAttractionInEvent = activeEvent.Atracciones.Any(a => a.Nombre == visit.Attraction.Nombre);

            if(isDuringEvent && isAttractionInEvent)
            {
                return eventoConfig.Puntos * basePoints;
            }
        }

        return basePoints;
    }
}
