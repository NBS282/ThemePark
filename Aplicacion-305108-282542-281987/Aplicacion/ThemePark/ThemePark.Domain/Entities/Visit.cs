using ThemePark.Exceptions;
namespace ThemePark.Entities;

public class Visit
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string AttractionName { get; private set; }
    public Attraction? Attraction { get; private set; }
    public DateTime EntryTime { get; private set; }
    public DateTime? ExitTime { get; private set; }
    public bool IsActive { get; private set; }
    public int Points { get; private set; }
    public string? ScoringStrategyName { get; private set; }

    public Visit(Guid userId, string attractionName, DateTime entryTime, int points = 0, string? scoringStrategyName = null)
    {
        if(userId == Guid.Empty)
        {
            throw new InvalidUserDataException("UserId", "vacío");
        }

        if(string.IsNullOrWhiteSpace(attractionName))
        {
            throw new InvalidAttractionDataException("AttractionName");
        }

        Id = Guid.NewGuid();
        UserId = userId;
        AttractionName = attractionName;
        Attraction = null;
        EntryTime = entryTime;
        ExitTime = null;
        IsActive = true;
        Points = points;
        ScoringStrategyName = scoringStrategyName;
    }

    public Visit(Guid userId, Attraction attraction, DateTime entryTime, int points = 0, string? scoringStrategyName = null)
    {
        if(userId == Guid.Empty)
        {
            throw new InvalidUserDataException("UserId", "vacío");
        }

        if(attraction == null)
        {
            throw new InvalidAttractionDataException("Attraction");
        }

        Id = Guid.NewGuid();
        UserId = userId;
        AttractionName = attraction.Nombre;
        Attraction = attraction;
        EntryTime = entryTime;
        ExitTime = null;
        IsActive = true;
        Points = points;
        ScoringStrategyName = scoringStrategyName;
    }

    public void MarkExit(DateTime exitTime)
    {
        ExitTime = exitTime;
        IsActive = false;
    }
}
