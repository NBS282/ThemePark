using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic.Attractions;

public class AttractionReportingService(IVisitRepository visitRepository) : IAttractionReportingService
{
    private readonly IVisitRepository _visitRepository = visitRepository;

    public Dictionary<string, int> GetUsageReport(DateTime fechaInicio, DateTime fechaFin)
    {
        if(fechaInicio > fechaFin)
        {
            throw BusinessLogicException.InvalidDateRange();
        }

        var visits = _visitRepository.GetVisitsByDateRange(fechaInicio, fechaFin);
        return visits
            .GroupBy(v => v.AttractionName)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
