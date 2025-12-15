using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess;
using ThemePark.ServiceFactory;
using ThemeParkApi.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .ConfigureModelValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddThemeParkServices(builder.Configuration);

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var dateTimeService = scope.ServiceProvider.GetRequiredService<IDateTimeBusinessLogic>();
}

var pluginLoader = app.Services.GetRequiredService<IPluginLoader>();
if(pluginLoader is ThemePark.BusinessLogic.ScoringStrategy.ScoringStrategyPluginLoader loader)
{
    var repository = app.Services.GetRequiredService<IScoringStrategyRepository>();
    loader.SetRepository(repository);

    var pluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
    loader.StartWatching(pluginsPath);
    Console.WriteLine($"Sistema de hot reload de plugins iniciado. Monitoreando: {pluginsPath}");
}

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
