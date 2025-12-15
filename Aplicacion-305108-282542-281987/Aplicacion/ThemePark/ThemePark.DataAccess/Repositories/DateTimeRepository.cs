using ThemePark.Entities;
using ThemePark.IDataAccess;

namespace ThemePark.DataAccess.Repositories;

public class DateTimeRepository(ThemeParkDbContext context) : IDateTimeRepository
{
    private readonly ThemeParkDbContext _context = context;

    public DateTime? GetCurrentDateTime()
    {
        var systemDateTime = _context.SystemDateTimes.FirstOrDefault();
        return systemDateTime?.CurrentDateTime;
    }

    public void SetCurrentDateTime(DateTime? dateTime)
    {
        var systemDateTime = _context.SystemDateTimes.FirstOrDefault();

        if(systemDateTime == null)
        {
            systemDateTime = new SystemDateTime { CurrentDateTime = dateTime };
            _context.SystemDateTimes.Add(systemDateTime);
        }
        else
        {
            systemDateTime.CurrentDateTime = dateTime;
        }

        _context.SaveChanges();
    }
}
