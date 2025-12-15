using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Users;

namespace ThemeParkApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthenticationFilter() : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            var headers = context.HttpContext.Request.Headers;

            if(!headers.ContainsKey("Authorization") || string.IsNullOrEmpty(headers["Authorization"]))
            {
                throw new MissingTokenException();
            }

            var authorizationHeader = headers["Authorization"].ToString();

            if(!IsValidTokenFormat(authorizationHeader))
            {
                throw new InvalidTokenFormatException();
            }

            var token = authorizationHeader.Substring("Bearer ".Length);
            var userDto = GetUserFromToken(token, context);

            if(userDto == null)
            {
                throw new InvalidTokenException();
            }

            context.HttpContext.Items["UserId"] = userDto.Id;
            context.HttpContext.Items["Token"] = token;
        }
        catch(Exception ex)
        {
            var (statusCode, title, message, innerCode) = ex switch
            {
                MissingTokenException exception => (401, "Token Faltante", exception.Message, "MissingToken"),
                InvalidTokenFormatException exception => (401, "Formato de Token Inválido", exception.Message, "InvalidTokenFormat"),
                InvalidTokenException exception => (401, "Token Inválido", exception.Message, "InvalidToken"),
                _ => (500, "Error Interno del Servidor", "Ocurrió un error inesperado", "InternalServerError")
            };

            context.Result = new ObjectResult(new
            {
                title = title,
                status = statusCode,
                message = message,
                innerCode = innerCode
            })
            {
                StatusCode = statusCode
            };
        }
    }

    private bool IsValidTokenFormat(string token)
    {
        return token.StartsWith("Bearer ");
    }

    private UserWithAllResponse? GetUserFromToken(string token, AuthorizationFilterContext context)
    {
        var authService = context.HttpContext.RequestServices.GetService<IAuthService>();
        var user = authService?.GetUserByToken(token);
        return user != null ? UserWithAllResponse.FromEntity(user) : null;
    }
}
