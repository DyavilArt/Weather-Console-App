using System;
using Weather_Console_App.Service;
using Weather_Console_App.Models;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var service = new WeatherService();
        while (true)
        {
            Console.Write("Введите название города (на английском): ");
            string? city = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(city))
            {
                Console.WriteLine("Название города не может быть пустым.");
                continue;
            }
            Console.WriteLine($"Поиск погоды для города '{city}'...");
            var result = await service.GetWeatherForCityAsync(city);
            if (result.City == null || result.Weather == null)
            {
                continue;
            }
            Console.WriteLine($"--- Погода в городе {result.City.Name} ---");
            Console.WriteLine($"Температура: {result.Weather.Temperature[0]}°C");
            Console.WriteLine($"Давление: {result.Weather.Pressure[0]} гПа");
            Console.WriteLine($"Код погоды: {result.Weather.WeatherCode[0]}");
            string description = GetWeatherDescription(result.Weather.WeatherCode[0]);
            Console.WriteLine($"Описание: {description}");
            Console.Write("Хотите проверить другой город? (y/n): ");
            string? choice = Console.ReadLine()?.ToLower();
            if (choice != "y")
                break;
        }
        Console.WriteLine("Программа завершена. Спасибо за использование!");
    }
    private static string GetWeatherDescription(int code)
    {
        return code switch
        {
            0 => "Ясно ☀️",
            1 => "Преимущественно ясно 🌤️",
            2 => "Переменная облачность ⛅",
            3 => "Пасмурно ☁️",
            45 or 48 => "Туман 🌫️",
            51 or 53 or 55 => "Морось 🌦️",
            61 or 63 or 65 => "Дождь 🌧️",
            71 or 73 or 75 => "Снег ❄️",
            80 or 81 or 82 => "Ливень ⛈️",
            95 or 96 or 99 => "Гроза ⚡",
            _ => "Неизвестно 🧐"
        };
    }
}
