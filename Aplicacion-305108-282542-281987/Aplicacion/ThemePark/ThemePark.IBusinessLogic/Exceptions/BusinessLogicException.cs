using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class BusinessLogicException(string message) : BaseCustomException(message, $"Business logic error: {message}")
{
    public static BusinessLogicException AttractionAlreadyExists(string name)
        => new($"Ya existe una atracción con el nombre '{name}'");

    public static BusinessLogicException AttractionNotFound(string name)
        => new($"No existe una atracción con el nombre '{name}'");

    public static BusinessLogicException AttractionOutOfService()
        => new("La atracción está fuera de servicio");

    public static BusinessLogicException AttractionCapacityReached()
        => new("La atracción ha alcanzado su capacidad máxima");

    public static BusinessLogicException InvalidAttractionType(string tipo)
        => new($"Tipo de atracción inválido: '{tipo}'. Los tipos válidos son: 0=MontañaRusa, 1=Simulador, 2=Espectaculo, 3=ZonaInteractiva");

    public static BusinessLogicException InvalidRoleType(string role)
        => new($"Tipo de rol inválido: '{role}'. Los roles válidos son: 0=OperadorAtraccion, 1=Visitante, 2=AdministradorParque");

    public static BusinessLogicException InvalidMembershipType(string membershipLevel)
        => new($"Tipo de nivelMembresia inválido: '{membershipLevel}'. Los valores válidos son: 0=Estándar, 1=Premium, 2=VIP");

    public static BusinessLogicException InvalidDateRange()
        => new("La fecha de inicio debe ser menor o igual a la fecha de fin");

    public static BusinessLogicException InvalidQRCode()
        => new("Código QR inválido");

    public static BusinessLogicException InvalidEntryType()
        => new("Tipo de entrada debe ser 'NFC' o 'QR'");

    public static BusinessLogicException NoValidTickets()
        => new("El usuario no tiene tickets válidos para la fecha actual");

    public static BusinessLogicException ActiveIncidentAlreadyExists(string attractionName)
        => new($"Ya existe una incidencia activa para la atracción '{attractionName}'");

    public static BusinessLogicException EventAlreadyExists(string name)
        => new($"Ya existe un evento con el nombre '{name}'");

    public static BusinessLogicException AttractionsNotFound(List<string> attractionNames)
        => new($"Las siguientes atracciones no existen: {string.Join(", ", attractionNames)}");

    public static BusinessLogicException UserNotExists()
        => new("Usuario no existe");

    public static BusinessLogicException EventNotExists()
        => new("Evento no existe");

    public static BusinessLogicException RewardAlreadyExists(string nombre)
        => new($"Ya existe una recompensa con el nombre '{nombre}'");

    public static BusinessLogicException RewardNotFound(int id)
        => new($"No existe una recompensa con el ID '{id}'");

    public static BusinessLogicException RewardInactive()
        => new("La recompensa no está activa");

    public static BusinessLogicException InsufficientPoints(int puntosDisponibles, int puntosRequeridos)
        => new($"Puntos insuficientes. Tienes {puntosDisponibles} puntos, pero necesitas {puntosRequeridos}");

    public static BusinessLogicException InsufficientMembershipLevel()
        => new("El nivel de membresía del usuario no cumple con el requerido para esta recompensa");

    public static BusinessLogicException MaintenanceNotFound(string maintenanceId)
        => new($"No existe un mantenimiento con el ID '{maintenanceId}'");
}
