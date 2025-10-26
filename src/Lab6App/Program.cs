using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab6App
{
    class Program
    {
        private const int RequiredCount = 50;
        static async Task<int> Main(string[] args)
        {
            Logger.Init();
            Console.WriteLine("Lab #6: LINQ & OpenWeatherMap demo");
            Logger.Log("Program started.");

            var apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? string.Empty;
            List<Weather> weathers;

            if (!string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Using live OpenWeatherMap API (OPENWEATHER_API_KEY provided).");
                weathers = await FetchWeathersLive(apiKey, RequiredCount);
            }
            else
            {
                Console.WriteLine("No API key provided. Using generated sample data.");
                weathers = GenerateSampleWeathers(RequiredCount);
            }

            Console.WriteLine($"\nCollected {weathers.Count} weather records.");
            Logger.Log($"Collected {weathers.Count} records.");

            if (weathers.Count == 0)
            {
                Console.WriteLine("No data collected. Exiting.");
                return 1;
            }

            var max = weathers.OrderByDescending(w => w.Temp).First();
            var min = weathers.OrderBy(w => w.Temp).First();
            Console.WriteLine($"\nMax temperature: {max.Temp:F1}°C in {max.Country} / {max.Name}");
            Console.WriteLine($"Min temperature: {min.Temp:F1}°C in {min.Country} / {min.Name}");
            Logger.Log($"Max {max.Temp:F1}C {max.Country}/{max.Name}; Min {min.Temp:F1}C {min.Country}/{min.Name}");

            var avg = weathers.Average(w => w.Temp);
            Console.WriteLine($"\nAverage temperature (collected points): {avg:F2}°C");
            Logger.Log($"Average temp: {avg:F2}C");

            var countryCount = weathers.Select(w => w.Country).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().Count();
            Console.WriteLine($"\nDistinct countries in collection: {countryCount}");
            Logger.Log($"Distinct countries: {countryCount}");

            var targetDescriptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "clear sky", "rain", "few clouds" };
            var found = weathers.FirstOrDefault(w => targetDescriptions.Contains(w.Description));
            if (!string.IsNullOrEmpty(found.Country))
            {
                Console.WriteLine($"\nFirst found with description in [clear sky, rain, few clouds]: {found.Country} / {found.Name} -> {found.Description}");
                Logger.Log($"Found special desc: {found.Country}/{found.Name} -> {found.Description}");
            }
            else
            {
                Console.WriteLine("\nNo entry found with description clear sky / rain / few clouds");
                Logger.Log("No special description found");
            }

            var groupByCountry = weathers
                .Where(w => !string.IsNullOrWhiteSpace(w.Country))
                .GroupBy(w => w.Country)
                .Select(g => new
                {
                    Country = g.Key,
                    AvgTemp = g.Average(w => w.Temp),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AvgTemp)
                .ToList();

            Console.WriteLine("\nAverage temperature per country (grouped):");
            foreach (var g in groupByCountry)
            {
                Console.WriteLine($"{g.Country}: avg = {g.AvgTemp:F2}°C (points = {g.Count})");
            }
            Logger.Log("Displayed grouped averages per country.");

            var top5Countries = groupByCountry.Take(5).ToList();
            Console.WriteLine("\nTop-5 warmest countries (by average temp, among collected points):");
            for (int i = 0; i < top5Countries.Count; i++)
            {
                var t = top5Countries[i];
                Console.WriteLine($"{i + 1}. {t.Country} — {t.AvgTemp:F2}°C (points = {t.Count})");
            }
            Logger.Log("Displayed top-5 warmest countries.");

            var top5Locations = weathers.OrderByDescending(w => w.Temp).Take(5).ToList();
            Console.WriteLine("\nTop-5 warmest locations (individual points):");
            for (int i = 0; i < top5Locations.Count; i++)
            {
                var t = top5Locations[i];
                Console.WriteLine($"{i + 1}. {t.Country}/{t.Name} — {t.Temp:F2}°C ({t.Description})");
            }
            Logger.Log("Displayed top-5 warmest locations.");

            Console.WriteLine("\nSample records:");
            foreach (var w in weathers.Take(10))
                Console.WriteLine(w);

            Logger.Log("Program finished.");
            return 0;
        }

        static async Task<List<Weather>> FetchWeathersLive(string apiKey, int count)
        {
            var rnd = new Random();
            var result = new List<Weather>();
            int attempts = 0;
            while (result.Count < count && attempts < count * 10)
            {
                attempts++;
                double lat = rnd.NextDouble() * 180.0 - 90.0;
                double lon = rnd.NextDouble() * 360.0 - 180.0;
                var w = await WeatherClient.FetchAsync(lat, lon, apiKey);
                if (w.HasValue)
                {
                    if (string.IsNullOrWhiteSpace(w.Value.Country) || string.IsNullOrWhiteSpace(w.Value.Name))
                        continue;
                    result.Add(w.Value);
                    Console.Write($"\rFetched: {result.Count}/{count}   ");
                }
            }
            Console.WriteLine();
            return result;
        }

        static List<Weather> GenerateSampleWeathers(int count)
        {
            var rnd = new Random(12345);
            var countries = new[] { "US", "RU", "GB", "FR", "DE", "CN", "IN", "BR", "AU", "JP" };
            var names = new[] { "CityA", "CityB", "CityC", "TownX", "PortY", "VillageZ", "Metro" };
            var descs = new[] { "clear sky", "few clouds", "scattered clouds", "broken clouds", "shower rain", "rain", "thunderstorm", "snow", "mist" };
            var list = new List<Weather>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new Weather
                {
                    Country = countries[rnd.Next(countries.Length)],
                    Name = names[rnd.Next(names.Length)] + i,
                    Temp = rnd.NextDouble() * 60.0 - 30.0,
                    Description = descs[rnd.Next(descs.Length)]
                });
            }
            return list;
        }
    }
}
