using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class ScoringStrategyException(string message) : BaseCustomException(message, $"Scoring strategy error: {message}")
{
    public static ScoringStrategyException NoActiveStrategy()
        => new("No se encontró una estrategia de puntuación activa");

    public static ScoringStrategyException NoActiveStrategyToDeactivate()
        => new("No hay ninguna estrategia activa para desactivar");

    public static ScoringStrategyException CannotActivateStrategy()
        => new("No se puede activar la estrategia: ya hay otra estrategia activa");

    public static ScoringStrategyException UnsupportedAlgorithmType(string? algorithmType)
        => new($"Tipo de algoritmo {algorithmType} no compatible");

    public static ScoringStrategyException NoTypedConfiguration()
        => new("La estrategia activa no tiene configuración tipada");

    public static ScoringStrategyException InvalidConfigurationType(string expectedType)
        => new($"La configuración debe ser de tipo {expectedType} para este algoritmo");

    public static ScoringStrategyException CannotDeleteActiveStrategy()
        => new("No se puede eliminar una estrategia de puntuación activa");
}
