namespace ThemePark.Exceptions;

public sealed class EventCapacityExceededException(string eventName, int maxCapacity) : BaseCustomException($"El evento '{eventName}' ha alcanzado su capacidad máxima ({maxCapacity} tickets vendidos)",
           $"Event '{eventName}' has reached maximum capacity ({maxCapacity} tickets sold)")
{
}
