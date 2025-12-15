using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ThemeParkApi.Filters;

public sealed class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        var mapping = ExceptionMappings.GetMapping(exceptionType);

        if(mapping.HasValue)
        {
            var message = exceptionType.Name == "JsonException"
                ? "El request contiene campos no reconocidos o tiene formato inválido"
                : context.Exception.Message;

            context.Result = new ObjectResult(new
            {
                title = mapping.Value.Title,
                status = mapping.Value.StatusCode,
                message = message
            })
            {
                StatusCode = mapping.Value.StatusCode
            };
        }
        else
        {
            var unmappedExceptionInfo = $"Unmapped exception: {exceptionType.FullName} - {context.Exception.Message}";
            Console.WriteLine(unmappedExceptionInfo);

            context.Result = new ObjectResult(new
            {
                title = "Error Interno del Servidor",
                status = 500,
                message = "Ocurrió un error inesperado",
                details = context.Exception.Message
            })
            {
                StatusCode = 500
            };
        }

        context.ExceptionHandled = true;
    }
}
