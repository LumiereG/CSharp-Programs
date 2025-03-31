# Weather Data Processor

This program is a demonstration of asynchronous programming techniques in C#, utilizing `async/await` and `Task` to efficiently fetch and process weather data for multiple cities. It uses a mock weather service to simulate the retrieval of temperature data, and then processes this data to calculate average temperatures, identify extreme weather days, and find the city with the highest average temperature.

## Features

- Fetches weather data for multiple cities asynchronously.
- Calculates the average temperature for each city.
- Identifies extreme weather days (temperatures below 0°C or above 30°C).
- Determines the city with the highest average temperature.
- Implements error handling for network issues, invalid input, and malformed data.
- Uses parallel tasks to improve performance.

## Technologies Used

- C# (.NET)
- `async/await` and `Task` for asynchronous programming
- `HttpClient` for making HTTP requests
- `JsonSerializer` for parsing JSON responses
- `LINQ` for data processing

## Requirements

- .NET Core or .NET 5/6 SDK
- Visual Studio or any C# development environment
