using ThemePark.Entities;

namespace ThemePark.BusinessLogic.Attractions;

public interface IAccessControlService
{
    Visit ValidateTicketAndRegisterAccess(string nombre, string tipoEntrada, string codigo);
    Visit RegisterExit(string nombre, string tipoEntrada, string codigo);
}
