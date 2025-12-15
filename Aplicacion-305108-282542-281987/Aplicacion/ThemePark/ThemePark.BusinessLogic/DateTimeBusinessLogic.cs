using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using static ThemePark.DateParser;

namespace ThemePark.BusinessLogic;

public class DateTimeBusinessLogic : IDateTimeBusinessLogic
{
    private readonly IDateTimeRepository _dateTimeRepository;

    public DateTimeBusinessLogic(IDateTimeRepository dateTimeRepository)
    {
        _dateTimeRepository = dateTimeRepository;

        var currentDateTime = _dateTimeRepository.GetCurrentDateTime();
        if(currentDateTime == null)
        {
            _dateTimeRepository.SetCurrentDateTime(DateTime.Now);
        }
    }

    public string GetCurrentDateTime()
    {
        var dateTime = _dateTimeRepository.GetCurrentDateTime()!.Value;
        return dateTime.ToString("yyyy-MM-ddTHH:mm");
    }

    public string SetCurrentDateTime(string fechaHora)
    {
        if(string.IsNullOrEmpty(fechaHora))
        {
            throw DateTimeBusinessLogicException.NullOrEmptyDateTime();
        }

        DateTime parsedDateTime;
        try
        {
            parsedDateTime = ParseDateTime(fechaHora, "fechaHora");
        }
        catch(InvalidRequestDataException)
        {
            throw DateTimeBusinessLogicException.InvalidFormat();
        }

        ValidateNotInThePast(parsedDateTime);

        _dateTimeRepository.SetCurrentDateTime(parsedDateTime);
        return fechaHora;
    }

    private void ValidateNotInThePast(DateTime newDateTime)
    {
        var currentSystemDateTime = _dateTimeRepository.GetCurrentDateTime();
        if(currentSystemDateTime.HasValue && newDateTime < currentSystemDateTime.Value)
        {
            throw DateTimeBusinessLogicException.CannotSetPastDate();
        }
    }
}
