using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Users;

namespace ThemeParkApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthorizationFilterAttribute(params string[] requiredRoles)
    : Attribute,
    IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(context.Result != null)
        {
            return;
        }

        try
        {
            var token = context.HttpContext.Items["Token"] as string;

            if(string.IsNullOrEmpty(token))
            {
                throw new MissingTokenException();
            }

            if(requiredRoles.Length > 0)
            {
                var authService = context.HttpContext.RequestServices.GetService<IAuthService>();
                var userEntity = authService?.GetUserByToken(token);

                if(userEntity == null)
                {
                    throw new MissingTokenException();
                }

                var userLoggedMapped = UserWithAllResponse.FromEntity(userEntity);
                var userHasRequiredRole = requiredRoles.Any(role => userLoggedMapped.Roles.Any(userRole => userRole == role));

                if(!userHasRequiredRole)
                {
                    throw new MissingRoleException(requiredRoles, userLoggedMapped.Roles.Distinct().ToArray());
                }
            }
        }
        catch(Exception ex)
        {
            var (statusCode, title, message) = ex switch
            {
                MissingTokenException exception => (401, "Token Faltante", exception.Message),
                MissingRoleException exception => (403, "Rol Faltante", exception.Message),
                _ => (500, "Error Interno del Servidor", "Ocurrió un error inesperado")
            };

            context.Result = new ObjectResult(new
            {
                title = title,
                status = statusCode,
                message = message
            })
            {
                StatusCode = statusCode
            };
        }
    }
}
