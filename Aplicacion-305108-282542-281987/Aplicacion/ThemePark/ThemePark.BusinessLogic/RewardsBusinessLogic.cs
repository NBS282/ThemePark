using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic;

public class RewardsBusinessLogic(
    IRewardRepository rewardRepository,
    IRewardExchangeRepository rewardExchangeRepository,
    IVisitRepository visitRepository,
    IUserRepository userRepository,
    ISessionRepository sessionRepository) : IRewardsBusinessLogic
{
    private readonly IRewardRepository _rewardRepository = rewardRepository;
    private readonly IRewardExchangeRepository _rewardExchangeRepository = rewardExchangeRepository;
    private readonly IVisitRepository _visitRepository = visitRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public Reward CreateReward(string nombre, string descripcion, int costoPuntos, int cantidadDisponible, int? nivelMembresiaRequerido)
    {
        if(_rewardRepository.ExistsByName(nombre))
        {
            throw BusinessLogicException.RewardAlreadyExists(nombre);
        }

        NivelMembresia? nivelMembresia = nivelMembresiaRequerido.HasValue
            ? (NivelMembresia)nivelMembresiaRequerido.Value
            : null;

        var reward = new Reward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresia);
        _rewardRepository.Save(reward);

        return reward;
    }

    public void DeleteReward(int id)
    {
        var reward = _rewardRepository.GetById(id);
        if(reward == null)
        {
            throw BusinessLogicException.RewardNotFound(id);
        }

        _rewardRepository.Delete(id);
    }

    public List<Reward> GetAllRewards()
    {
        return _rewardRepository.GetAll();
    }

    public Reward GetRewardById(int id)
    {
        var reward = _rewardRepository.GetById(id);
        if(reward == null)
        {
            throw BusinessLogicException.RewardNotFound(id);
        }

        return reward;
    }

    public Reward UpdateReward(int id, string? descripcion, int? costoPuntos, int? cantidadDisponible, int? nivelMembresiaRequerido)
    {
        var reward = _rewardRepository.GetById(id);
        if(reward == null)
        {
            throw BusinessLogicException.RewardNotFound(id);
        }

        var descripcionFinal = descripcion ?? reward.Descripcion;
        var costoFinal = costoPuntos ?? reward.CostoPuntos;
        var cantidadFinal = cantidadDisponible ?? reward.CantidadDisponible;
        NivelMembresia? nivelFinal = nivelMembresiaRequerido.HasValue
            ? (NivelMembresia)nivelMembresiaRequerido.Value
            : reward.NivelMembresiaRequerido;

        reward.UpdateInfo(descripcionFinal, costoFinal, cantidadFinal, nivelFinal);
        _rewardRepository.Save(reward);

        return reward;
    }

    public List<Reward> GetAvailableRewardsForUser()
    {
        var userId = _sessionRepository.GetCurrentUserId();
        var userVisits = _visitRepository.GetByUserId(userId);
        var puntosAcumulados = userVisits.Sum(v => v.Points);

        var previousExchanges = _rewardExchangeRepository.GetByUserId(userId);
        var puntosGastados = previousExchanges.Sum(e => e.PuntosDescontados);

        var puntosDisponibles = puntosAcumulados - puntosGastados;

        return _rewardRepository.GetAll()
            .Where(r => r.Activa && r.CantidadDisponible > 0 && r.CostoPuntos <= puntosDisponibles)
            .ToList();
    }

    public RewardExchange ExchangeReward(int id)
    {
        var reward = _rewardRepository.GetById(id);
        if(reward == null)
        {
            throw BusinessLogicException.RewardNotFound(id);
        }

        if(!reward.Activa)
        {
            throw BusinessLogicException.RewardInactive();
        }

        var userId = _sessionRepository.GetCurrentUserId();
        var userVisits = _visitRepository.GetByUserId(userId);
        var puntosAcumulados = userVisits.Sum(v => v.Points);

        var previousExchanges = _rewardExchangeRepository.GetByUserId(userId);
        var puntosGastados = previousExchanges.Sum(e => e.PuntosDescontados);

        var puntosDisponibles = puntosAcumulados - puntosGastados;

        if(puntosDisponibles < reward.CostoPuntos)
        {
            throw BusinessLogicException.InsufficientPoints(puntosDisponibles, reward.CostoPuntos);
        }

        if(reward.NivelMembresiaRequerido.HasValue)
        {
            var user = _userRepository.GetById(userId);
            var userMembership = user.GetMembresia();

            if(userMembership < reward.NivelMembresiaRequerido.Value)
            {
                throw BusinessLogicException.InsufficientMembershipLevel();
            }
        }

        reward.DecrementarStock();

        var puntosRestantes = puntosDisponibles - reward.CostoPuntos;
        var exchange = new RewardExchange(id, userId, reward.CostoPuntos, puntosRestantes);

        _rewardExchangeRepository.Save(exchange);
        _rewardRepository.Save(reward);

        return exchange;
    }

    public List<RewardExchange> GetUserExchanges()
    {
        var userId = _sessionRepository.GetCurrentUserId();
        return _rewardExchangeRepository.GetByUserId(userId);
    }
}
