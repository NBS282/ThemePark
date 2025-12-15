using Microsoft.AspNetCore.Http;
using ThemePark.Entities;
using ThemePark.IDataAccess;

namespace ThemePark.DataAccess.Repositories;

public class SessionRepository(ThemeParkDbContext context, IHttpContextAccessor httpContextAccessor) : ISessionRepository
{
    private readonly ThemeParkDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public void Add(Session session)
    {
        _context.Sessions.Add(session);
        _context.SaveChanges();
    }

    public Session? GetByToken(string token)
    {
        return _context.Sessions.FirstOrDefault(s => s.Token == token);
    }

    public Guid GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.Items["UserId"];
        if(userId == null)
        {
            throw new UnauthorizedAccessException("Usuario no autenticado");
        }

        return (Guid)userId;
    }
}
