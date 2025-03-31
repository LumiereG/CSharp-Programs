using HttpSimulator;
using System.Text.Json;
using System.Globalization;

namespace Lab12;

internal class Program
{
    static async Task Main(string[] args)
    {
        var cities = new[] { "New York", "London", "Tokyo", "Sydney", "Berlin" };

        // Fetching weather data for each city in parallel
        var tasks = cities.Select(city => WeatherProcessor.FetchAndProcessWeatherDataAsync(city));
        var cityWeatherData = await Task.WhenAll(tasks);

        // Processing weather data for each city
        var processTasks = cityWeatherData.Select(async cityWeather =>
        {
            // Calculating average temperature and handling invalid data
            var averageTemperature = await WeatherProcessor.CalculateAverageTemperatureAsync(cityWeather.DailyTemperatures) ?? WeatherProcessor.InvalidTemperatureIndicator;

            // Finding extreme weather days (temperature < 0°C or > 30°C)
            var extremeWeatherDays = await WeatherProcessor.FindExtremeWeatherDaysAsync(cityWeather.DailyTemperatures);

            return new ProcessedCityWeather
            {
                City = cityWeather.City,
                AverageTemperature = averageTemperature,
                ExtremeWeatherDays = extremeWeatherDays.Any() ? extremeWeatherDays : new List<string> { "No extreme weather days" }
            };
        });

        // Wait for all processing tasks to complete
        var processedResults = await Task.WhenAll(processTasks);

        // Display processed weather data for each city
        foreach (var result in processedResults)
        {
            Console.WriteLine($"City: {result.City}");
            if (result.AverageTemperature == WeatherProcessor.InvalidTemperatureIndicator)
            {
                Console.WriteLine("Average Temperature: No data available");
            }
            else
            {
                Console.WriteLine($"Average Temperature: {result.AverageTemperature:F1}°C");
            }
            Console.WriteLine("Extreme Weather Days:\n" +
             string.Join("\n", result.ExtremeWeatherDays));
            Console.WriteLine();
        }

    }

}
