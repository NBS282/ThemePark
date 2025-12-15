using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class CannotModifyFounderAdminException()
    : BaseCustomException(
        "No se puede eliminar o modificar el rol de administrador del usuario fundador",
        "Cannot delete or modify the founder administrator user")
{
}
