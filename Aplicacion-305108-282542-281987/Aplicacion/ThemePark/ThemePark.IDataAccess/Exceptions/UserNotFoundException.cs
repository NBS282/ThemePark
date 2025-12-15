using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class UserNotFoundException : BaseCustomException
{
    public UserNotFoundException(Guid userId)
        : base($"Usuario con ID {userId} no encontrado",
               $"User with ID {userId} not found")
    {
    }

    public UserNotFoundException(string codigoIdentificacion)
        : base($"Usuario con código identificación {codigoIdentificacion} no encontrado",
               $"User with identification code {codigoIdentificacion} not found")
    {
    }
}
