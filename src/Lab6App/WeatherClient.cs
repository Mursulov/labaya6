using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lab6App
{
    public static class WeatherClient
    {
        private static readonly HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

        public static async Task<Weather?> FetchAsync(double lat, double lon, string apiKey)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}";
                using var resp = await client.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return null;
                using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                if (!root.TryGetProperty("sys", out var sys) || !sys.TryGetProperty("country", out var countryEl)) return null;
                if (!root.TryGetProperty("name", out var nameEl)) return null;
                if (!root.TryGetProperty("main", out var main) || !main.TryGetProperty("temp", out var tempEl)) return null;
                if (!root.TryGetProperty("weather", out var weatherArr) || weatherArr.GetArrayLength() == 0) return null;

                var country = countryEl.GetString() ?? "";
                var name = nameEl.GetString() ?? "";
                var tempK = tempEl.GetDouble();
                var tempC = tempK - 273.15;
                var description = weatherArr[0].GetProperty("description").GetString() ?? "";

                return new Weather
                {
                    Country = country,
                    Name = name,
                    Temp = tempC,
                    Description = description
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
