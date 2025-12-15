using ThemePark.Exceptions;

namespace ThemePark.Entities;

public class RewardExchange
{
    public int Id { get; private set; }
    public int RewardId { get; private set; }
    public Guid UserId { get; private set; }
    public int PuntosDescontados { get; private set; }
    public int PuntosRestantesUsuario { get; private set; }
    public DateTime FechaCanje { get; private set; }
    public string Estado { get; private set; }

    public RewardExchange(int rewardId, Guid userId, int puntosDescontados, int puntosRestantesUsuario)
    {
        if(rewardId <= 0)
        {
            throw new InvalidRequestDataException("El ID de la recompensa debe ser mayor a 0");
        }

        if(userId == Guid.Empty)
        {
            throw new InvalidRequestDataException("El ID del usuario no puede estar vacío");
        }

        if(puntosDescontados <= 0)
        {
            throw new InvalidRequestDataException("Los puntos descontados deben ser mayores a 0");
        }

        if(puntosRestantesUsuario < 0)
        {
            throw new InvalidRequestDataException("Los puntos restantes no pueden ser negativos");
        }

        RewardId = rewardId;
        UserId = userId;
        PuntosDescontados = puntosDescontados;
        PuntosRestantesUsuario = puntosRestantesUsuario;
        Estado = "Confirmado";
        FechaCanje = DateTime.Now;
    }
}
