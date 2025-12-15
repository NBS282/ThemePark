using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThemePark.BusinessLogic;
using ThemePark.BusinessLogic.Attractions;
using ThemePark.BusinessLogic.ScoringStrategy;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess;

namespace ThemePark.ServiceFactory;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddThemeParkServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddDbContext<ThemeParkDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAttractionManagementService, AttractionManagementService>();
        services.AddScoped<IAccessControlService, AccessControlService>();
        services.AddScoped<IIncidentManagementService, IncidentManagementService>();
        services.AddScoped<IAttractionReportingService, AttractionReportingService>();
        services.AddScoped<IMaintenanceManagementService, MaintenanceManagementService>();

        services.AddScoped<IAttractionsBusinessLogic, AttractionsBusinessLogic>();

        services.AddScoped<IDateTimeBusinessLogic, DateTimeBusinessLogic>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEventsService, EventService>();
        services.AddScoped<IScoringStrategyService, ScoringStrategyService>();
        services.AddScoped<IScoringAlgorithmFactory, ScoringAlgorithmFactory>();
        services.AddSingleton<IPluginLoader, ScoringStrategyPluginLoader>();
        services.AddScoped<ITicketsBusinessLogic, TicketsBusinessLogic>();
        services.AddScoped<IScoringStrategyRepository, ScoringStrategyRepository>();
        services.AddScoped<IRankingService, RankingService>();
        services.AddScoped<IRewardsBusinessLogic, RewardsBusinessLogic>();
        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<IDateTimeRepository, DateTimeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IVisitRepository, VisitRepository>();
        services.AddScoped<IIncidentRepository, IncidentRepository>();
        services.AddScoped<ITicketsRepository, TicketsRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IRewardExchangeRepository, RewardExchangeRepository>();

        return services;
    }

    public static IMvcBuilder ConfigureModelValidation(this IMvcBuilder mvcBuilder)
    {
        return mvcBuilder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray());

                return new ObjectResult(new
                {
                    title = "Error de Validación",
                    status = 400,
                    message = "El request contiene errores de validación",
                    errors = errors
                })
                {
                    StatusCode = 400
                };
            };
        });
    }
}
