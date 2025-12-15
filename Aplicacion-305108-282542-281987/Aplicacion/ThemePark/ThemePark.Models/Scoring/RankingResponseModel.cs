using ThemePark.Entities;

namespace ThemePark.Models.Scoring;

public class RankingResponseModel
{
    public int Posicion { get; set; }
    public UserRankingModel Usuario { get; set; } = new();
    public int Puntos { get; set; }

    public static RankingResponseModel FromEntity(UserRanking userRanking)
    {
        return new RankingResponseModel
        {
            Posicion = userRanking.Posicion,
            Usuario = new UserRankingModel
            {
                Nombre = userRanking.Usuario.Nombre,
                Apellido = userRanking.Usuario.Apellido,
                Email = userRanking.Usuario.Email
            },
            Puntos = userRanking.Puntos
        };
    }
}

public class UserRankingModel
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
