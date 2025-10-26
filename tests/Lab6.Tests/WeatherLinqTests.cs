using NUnit.Framework;
using Lab6App;
using System.Collections.Generic;
using System.Linq;

namespace Lab6.Tests
{
    public class WeatherLinqTests
    {
        private List<Weather> CreateSample()
        {
            return new List<Weather>
            {
                new Weather { Country = "A", Name = "n1", Temp = 10, Description = "clear sky" },
                new Weather { Country = "B", Name = "n2", Temp = 20, Description = "rain" },
                new Weather { Country = "A", Name = "n3", Temp = -5, Description = "few clouds" },
                new Weather { Country = "C", Name = "n4", Temp = 0, Description = "mist" },
            };
        }

        [Test]
        public void MaxAndMinTemp()
        {
            var data = CreateSample();
            var max = data.OrderByDescending(w => w.Temp).First();
            var min = data.OrderBy(w => w.Temp).First();
            Assert.AreEqual(20, max.Temp);
            Assert.AreEqual(-5, min.Temp);
        }

        [Test]
        public void AverageTemp()
        {
            var data = CreateSample();
            var avg = data.Average(w => w.Temp);
            Assert.AreEqual((10 + 20 - 5 + 0)/4.0, avg);
        }

        [Test]
        public void DistinctCountriesCount()
        {
            var data = CreateSample();
            var c = data.Select(w => w.Country).Distinct().Count();
            Assert.AreEqual(3, c);
        }

        [Test]
        public void FindByDescription()
        {
            var data = CreateSample();
            var targets = new HashSet<string>(new[] { "clear sky", "rain", "few clouds" }, System.StringComparer.OrdinalIgnoreCase);
            var found = data.FirstOrDefault(w => targets.Contains(w.Description));
            Assert.IsNotNull(found);
            Assert.IsTrue(targets.Contains(found.Description));
        }
    }
}
