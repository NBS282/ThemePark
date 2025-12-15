using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IVisitRepository
{
    Visit Save(Visit visit);
    Visit Update(Visit visit);
    Visit GetById(Guid visitId);
    List<Visit> GetByUserId(Guid userId);
    List<Visit> GetActiveVisitsByAttraction(string attractionName);
    Visit? GetActiveVisitByUserAndAttraction(Guid userId, string attractionName);
    Visit? GetActiveVisitByUser(Guid userId);
    List<Visit> GetVisitsByDate(DateTime date);
    List<Visit> GetVisitsByDateRange(DateTime fechaInicio, DateTime fechaFin);
    List<Visit> GetVisitsByUserAndDate(Guid userId, DateTime date);
}
