namespace ThemePark.Exceptions;

public sealed class CapacityExceededException(string attractionName, int currentCapacity, int maxCapacity) : BaseCustomException($"La atracción '{attractionName}' ha alcanzado su capacidad máxima ({currentCapacity}/{maxCapacity})",
           $"Attraction '{attractionName}' has reached maximum capacity ({currentCapacity}/{maxCapacity})")
{
}
