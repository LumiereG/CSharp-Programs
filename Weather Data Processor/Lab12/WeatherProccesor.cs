using HttpSimulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab12
{
    public static class WeatherProcessor
    {
        // Constant for invalid temperature value
        public const double InvalidTemperatureIndicator = -999.99;

        // Method to fetch and process weather data for a specific city
        public static async Task<CityWeather> FetchAndProcessWeatherDataAsync(string city)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(city))
                    throw new ArgumentException("City name cannot be empty or null.");

                using var client = new HttpClient(MockHttpMessageHandlerSingleton.Instance);
                var apiUrl = $"https://127.0.0.1:2137/api/v13/forecast?city={city}&daily=temperature";

                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch data for {city}: {response.StatusCode}");
                    return new CityWeather { City = city, DailyTemperatures = new List<double>() };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(jsonResponse);

                if (weatherData == null || weatherData.Daily?.Temperature == null)
                {
                    Console.WriteLine($"Invalid data for {city}.");
                    return new CityWeather { City = city, DailyTemperatures = new List<double>() };
                }

                return new CityWeather
                {
                    City = city,
                    DailyTemperatures = weatherData.Daily.Temperature
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error for {city}: {ex.Message}");
                return new CityWeather { City = city, DailyTemperatures = new List<double>() };
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Malformed JSON for {city}: {ex.Message}");
                return new CityWeather { City = city, DailyTemperatures = new List<double>() };
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Invalid input for {city}: {ex.Message}");
                return new CityWeather { City = city, DailyTemperatures = new List<double>() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data for {city}: {ex.Message}");
                return new CityWeather { City = city, DailyTemperatures = new List<double>() };
            }
        }

        // Method to calculate the average temperature for a list of temperatures
        public static async Task<double?> CalculateAverageTemperatureAsync(List<double> temperatures)
        {
            if (temperatures == null || temperatures.Count == 0)
                return null;

            return await Task.Run(() => temperatures.Average());
        }

        // Method to find extreme weather days (temperatures below 0°C or above 30°C)
        public static async Task<List<string>> FindExtremeWeatherDaysAsync(List<double> temperatures)
        {
            if (temperatures == null || temperatures.Count == 0)
                return new List<string> { "No temperature data available" };

            return await Task.Run(() =>
                temperatures.Select((temp, index) =>
                    temp < 0 || temp > 30 ? $"Day {index + 1}: {temp}°C" : null)
                .Where(day => day != null)
                .ToList());
        }



    }
}
