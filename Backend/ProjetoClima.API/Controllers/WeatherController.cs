using Microsoft.AspNetCore.Mvc;
using ProjetoClima.API.Models;
using ProjetoClima.API.Services;

namespace ProjetoClima.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }
        
        [HttpGet("{cidade}")]
        public async Task<ActionResult<WeatherResponse>> GetWeather(string cidade)
        {
            if (string.IsNullOrWhiteSpace(cidade))
            {
                return BadRequest(new { message = "Nome da cidade é obrigatório" });
            }

            try
            {
                _logger.LogInformation($"Buscando clima para: {cidade}");
                
                var weather = await _weatherService.GetWeatherAsync(cidade);
                
                if (weather == null)
                {
                    return NotFound(new { message = $"Cidade '{cidade}' não encontrada" });
                }

                return Ok(weather);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar clima para {cidade}");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}