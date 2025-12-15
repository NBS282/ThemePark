namespace ThemeParkApi.Filters;

public static class ExceptionMappings
{
    public static (int StatusCode, string Title)? GetMapping(Type exceptionType)
    {
        return exceptionType.Name switch
        {
            "MissingTokenException" => (401, "Token Faltante"),
            "InvalidTokenFormatException" => (401, "Formato de Token Inválido"),
            "InvalidTokenException" => (401, "Token Inválido"),
            "InvalidCredentialsException" => (401, "Credenciales Inválidas"),
            "InvalidAuthenticationException" => (401, "Error de Autenticación"),

            "MissingRoleException" => (403, "Rol Faltante"),
            "CannotModifyFounderAdminException" => (403, "Operación No Permitida"),
            "ForbiddenException" => (403, "Acceso Prohibido"),

            "BusinessLogicException" => (400, "Error de Lógica de Negocio"),
            "DateTimeBusinessLogicException" => (400, "Error de Validación de Fecha"),
            "ScoringStrategyException" => (400, "Error de Estrategia de Puntuación"),
            "InvalidRequestDataException" => (400, "Error de Validación"),
            "InvalidAttractionDataException" => (400, "Datos de Atracción Inválidos"),
            "InvalidEventDataException" => (400, "Datos de Evento Inválidos"),
            "InvalidIncidentDataException" => (400, "Datos de Incidencia Inválidos"),
            "InvalidMaintenanceDataException" => (400, "Datos de Mantenimiento Inválidos"),
            "InvalidTicketException" => (400, "Ticket Inválido"),
            "InvalidTimeFormatException" => (400, "Formato de Hora Inválido"),
            "InvalidUserDataException" => (400, "Datos de Usuario Inválidos"),

            "ActiveIncidentException" => (409, "Incidencia Activa"),
            "CannotDeleteAttractionException" => (409, "Atracción con Registros Asociados"),
            "CannotDeleteAttractionWithActiveIncidentsException" => (409, "Atracción con Incidencias Activas"),
            "CannotDeleteAttractionWithScheduledMaintenancesException" => (409, "Atracción con Mantenimientos Programados"),
            "CapacityExceededException" => (409, "Capacidad Excedida"),
            "EventCapacityExceededException" => (409, "Aforo Completo"),
            "VisitAlreadyActiveException" => (409, "Visita Ya Activa"),
            "UserAlreadyInAttractionException" => (409, "Usuario en Otra Atracción"),
            "UserRegistrationException" => (409, "Email Duplicado"),

            "ExpiredTicketException" => (410, "Ticket Expirado"),
            "TicketAlreadyUsedException" => (410, "Ticket Ya Utilizado"),

            "WrongAttractionException" => (422, "Ticket para Otra Atracción"),
            "TicketNotValidForDateException" => (422, "Ticket No Válido para la Fecha"),
            "EventTicketNotValidForTimeException" => (422, "Ticket de Evento Fuera de Horario"),
            "AgeLimitException" => (422, "Edad Mínima No Cumplida"),
            "NoActiveVisitException" => (422, "Sin Visita Activa"),

            "AttractionNotFoundException" => (404, "Atracción No Encontrada"),
            "EventNotFoundException" => (404, "Evento No Encontrado"),
            "IncidentNotFoundException" => (404, "Incidencia No Encontrada"),
            "ScoringStrategyNotFoundException" => (404, "Estrategia de Puntuación No Encontrada"),
            "TicketNotFoundException" => (404, "Ticket No Encontrado"),
            "UserNotFoundException" => (404, "Usuario No Encontrado"),
            "VisitNotFoundException" => (404, "Visita No Encontrada"),

            "JsonException" => (400, "Error de Formato JSON"),

            _ => null
        };
    }
}
