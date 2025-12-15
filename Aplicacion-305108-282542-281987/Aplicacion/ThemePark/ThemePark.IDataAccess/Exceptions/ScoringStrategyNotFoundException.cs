using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class ScoringStrategyNotFoundException(string nombre) : BaseCustomException($"Estrategia de puntuación '{nombre}' no encontrada",
           $"Scoring strategy '{nombre}' not found")
{
}
