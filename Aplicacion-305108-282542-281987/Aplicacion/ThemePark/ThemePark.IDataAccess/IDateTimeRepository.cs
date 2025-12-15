namespace ThemePark.IDataAccess;

public interface IDateTimeRepository
{
    DateTime? GetCurrentDateTime();
    void SetCurrentDateTime(DateTime? dateTime);
}
