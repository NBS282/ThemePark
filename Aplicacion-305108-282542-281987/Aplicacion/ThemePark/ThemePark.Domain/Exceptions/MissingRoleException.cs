namespace ThemePark.Exceptions;

public sealed class MissingRoleException(string[] requiredRoles, string[] currentRoles) : BaseCustomException($"El usuario no tiene los permisos necesarios. Roles requeridos: {string.Join(", ", requiredRoles)}. Roles actuales: {string.Join(", ", currentRoles)}",
           $"User does not have required permissions. Required roles: {string.Join(", ", requiredRoles)}. Current roles: {string.Join(", ", currentRoles)}")
{
}
