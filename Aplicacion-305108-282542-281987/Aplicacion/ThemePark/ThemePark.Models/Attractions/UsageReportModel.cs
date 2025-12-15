namespace ThemePark.Models.Attractions;

public class UsageReportModel
{
    public string AtraccionNombre { get; set; } = string.Empty;
    public int CantidadVisitas { get; set; }

    public static List<UsageReportModel> FromDictionary(Dictionary<string, int> usageData)
    {
        return usageData.Select(kvp => new UsageReportModel
        {
            AtraccionNombre = kvp.Key,
            CantidadVisitas = kvp.Value
        }).ToList();
    }
}
