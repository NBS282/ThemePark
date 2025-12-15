using ThemePark.Enums;

namespace ThemePark.IBusinessLogic;

public interface IScoringAlgorithmFactory
{
    IScoringAlgorithm CreateAlgorithm(TipoEstrategia? tipoEstrategia);
    IScoringAlgorithm CreateAlgorithm(Entities.ScoringStrategy strategy);
}
