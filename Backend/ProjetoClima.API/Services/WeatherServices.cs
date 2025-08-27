using ProjetoClima.API.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjetoClima.API.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherService> _logger;
        private readonly string _apiKey = "b7b06698670aaef621f81d0d66c7a8fe";

        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string cidade)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("OpenWeatherMap");
                
                var url = $"data/2.5/weather?q={cidade}&appid={_apiKey}&units=metric&lang=pt_br";
                
                _logger.LogInformation($"Fazendo requisição para: {url}");
                
                var response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Falha na requisição: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation($"Resposta recebida: {jsonContent}");
                
                var openWeatherResponse = JsonSerializer.Deserialize<OpenWeatherCurrentResponse>(
                    jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                
                if (openWeatherResponse == null)
                {
                    _logger.LogWarning("Não foi possível deserializar a resposta da API");
                    return null;
                }
                
                var main = openWeatherResponse.Main;
                var sys = openWeatherResponse.Sys;
                var wind = openWeatherResponse.Wind;
                var clouds = openWeatherResponse.Clouds;

                var weatherResponse = new WeatherResponse
                {
                    Nome = string.IsNullOrWhiteSpace(openWeatherResponse.Name) ? (cidade ?? "") : openWeatherResponse.Name,
                    Dt = openWeatherResponse.Dt,
                    Sunrise = sys?.Sunrise ?? 0,
                    Sunset = sys?.Sunset ?? 0,
                    Temp = main?.Temp ?? 0,
                    FeelsLike = main?.FeelsLike ?? 0,
                    Pressure = main?.Pressure ?? 0,
                    Humidity = main?.Humidity ?? 0,
                    DewPoint = 0, 
                    Uvi = 0,      
                    Clouds = clouds?.All ?? 0,
                    Visibility = openWeatherResponse.Visibility,
                    WindSpeed = wind?.Speed ?? 0,
                    WindDeg = wind?.Deg ?? 0,
                    WindGust = wind?.Gust ?? 0,
                    Weather = (openWeatherResponse.Weather ?? Array.Empty<OpenWeatherCurrentResponse.WeatherItem>())
                        .Select(x => new WeatherResponse.WeatherInfo
                        {
                            Id = x.Id,
                            Main = x.Main,
                            Description = string.IsNullOrEmpty(x.Description)
                                ? ""
                                : char.ToUpper(x.Description[0]) + (x.Description.Length > 1 ? x.Description[1..] : ""),
                            Icon = x.Icon
                        }).ToArray()
                };

                return weatherResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao chamar a API do OpenWeatherMap");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro ao deserializar resposta da API");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar dados do clima");
                return null;
            }
        }

        
        private sealed class OpenWeatherCurrentResponse
        {
            [JsonPropertyName("dt")]
            public int Dt { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("sys")]
            public SysInfo? Sys { get; set; }

            [JsonPropertyName("main")]
            public MainInfo? Main { get; set; }

            [JsonPropertyName("wind")]
            public WindInfo? Wind { get; set; }

            [JsonPropertyName("clouds")]
            public CloudsInfo? Clouds { get; set; }

            [JsonPropertyName("visibility")]
            public int Visibility { get; set; }

            [JsonPropertyName("weather")]
            public WeatherItem[]? Weather { get; set; }

            public sealed class SysInfo
            {
                [JsonPropertyName("sunrise")]
                public int Sunrise { get; set; }

                [JsonPropertyName("sunset")]
                public int Sunset { get; set; }
            }

            public sealed class MainInfo
            {
                [JsonPropertyName("temp")]
                public double Temp { get; set; }

                [JsonPropertyName("feels_like")]
                public double FeelsLike { get; set; }

                [JsonPropertyName("pressure")]
                public int Pressure { get; set; }

                [JsonPropertyName("humidity")]
                public int Humidity { get; set; }
            }

            public sealed class WindInfo
            {
                [JsonPropertyName("speed")]
                public double Speed { get; set; }

                [JsonPropertyName("deg")]
                public int Deg { get; set; }

                [JsonPropertyName("gust")]
                public double? Gust { get; set; }
            }

            public sealed class CloudsInfo
            {
                [JsonPropertyName("all")]
                public int All { get; set; }
            }

            public sealed class WeatherItem
            {
                [JsonPropertyName("id")]
                public int Id { get; set; }

                [JsonPropertyName("main")]
                public string Main { get; set; } = "";

                [JsonPropertyName("description")]
                public string Description { get; set; } = "";

                [JsonPropertyName("icon")]
                public string Icon { get; set; } = "";
            }
        }
    }
}