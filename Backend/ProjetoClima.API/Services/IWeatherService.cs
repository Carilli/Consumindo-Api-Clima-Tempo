using ProjetoClima.API.Models;

namespace ProjetoClima.API.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> GetWeatherAsync(string cidade);
    }
}