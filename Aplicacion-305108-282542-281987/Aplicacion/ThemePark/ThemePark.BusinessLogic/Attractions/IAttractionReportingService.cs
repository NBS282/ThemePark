namespace ThemePark.BusinessLogic.Attractions;

public interface IAttractionReportingService
{
    Dictionary<string, int> GetUsageReport(DateTime fechaInicio, DateTime fechaFin);
}
