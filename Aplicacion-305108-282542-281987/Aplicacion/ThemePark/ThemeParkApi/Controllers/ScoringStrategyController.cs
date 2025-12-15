using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.ScoringStrategy;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("scoring/strategies")]
public class ScoringStrategyController(IScoringStrategyService scoringStrategyService, IPluginLoader pluginLoader) : ControllerBase
{
    private readonly IScoringStrategyService _scoringStrategyService = scoringStrategyService;
    private readonly IPluginLoader _pluginLoader = pluginLoader;

    [HttpGet]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult GetStrategies()
    {
        return Ok(ScoringStrategyDto.FromEntities(_scoringStrategyService.GetAllStrategies()));
    }

    [HttpGet("{nombre}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult GetStrategy(string nombre)
    {
        return Ok(ScoringStrategyDto.FromEntity(_scoringStrategyService.GetByName(nombre)));
    }

    [HttpPost]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult CreateStrategy([FromBody] CreateScoringStrategyDto createDto)
    {
        var createdDto = ScoringStrategyDto.FromEntity(
            _scoringStrategyService.CreateStrategy(
                createDto.Nombre,
                createDto.Descripcion,
                CreateScoringStrategyDto.MapToEntity(createDto.Algoritmo),
                createDto.Configuracion?.ToEntity(),
                createDto.PluginTypeIdentifier,
                createDto.ConfigurationJson));

        return CreatedAtAction(nameof(GetStrategy), new { nombre = createdDto.Nombre }, createdDto);
    }

    [HttpPatch("{nombre}/activate")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult ToggleActiveStrategy(string nombre)
    {
        return Ok(ScoringStrategyDto.FromEntity(_scoringStrategyService.ToggleActive(nombre)));
    }

    [HttpPatch("deactivate")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult DeactivateStrategy()
    {
        _scoringStrategyService.Deactivate();
        return Ok();
    }

    [HttpPatch("{nombre}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult UpdateStrategy(string nombre, [FromBody] UpdateScoringStrategyDto updateDto)
    {
        return Ok(ScoringStrategyDto.FromEntity(_scoringStrategyService.Update(nombre, updateDto.ToEntity())));
    }

    [HttpDelete("{nombre}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult DeleteStrategy(string nombre)
    {
        _scoringStrategyService.Delete(nombre);
        return NoContent();
    }

    [HttpGet("available-types")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult GetAvailableTypes()
    {
        var types = _scoringStrategyService.GetAvailableStrategyTypes();
        return Ok(types);
    }

    [HttpGet("config-schema/{typeIdentifier}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult GetConfigSchema(string typeIdentifier)
    {
        var plugin = _pluginLoader.GetPlugin(typeIdentifier);
        if(plugin != null)
        {
            var schema = plugin.GetConfigurationSchema();
            return Ok(new
            {
                IsPlugin = true,
                TypeIdentifier = plugin.StrategyTypeIdentifier,
                Name = plugin.StrategyName,
                Description = plugin.Description,
                Schema = schema
            });
        }

        return Ok(new
        {
            IsPlugin = false,
            TypeIdentifier = typeIdentifier,
            Schema = new { }
        });
    }
}
