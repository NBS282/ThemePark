namespace ThemePark.Entities;

public class UserRanking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Posicion { get; set; }
    public User Usuario { get; set; } = null!;
    public int Puntos { get; set; }
    public DateTime Fecha { get; set; }

    public UserRanking()
    {
    }

    public UserRanking(int posicion, User usuario, int puntos)
    {
        Posicion = posicion;
        Usuario = usuario;
        Puntos = puntos;
    }

    public UserRanking(int posicion, User usuario, int puntos, DateTime fecha)
    {
        Posicion = posicion;
        Usuario = usuario;
        Puntos = puntos;
        Fecha = fecha;
    }
}
