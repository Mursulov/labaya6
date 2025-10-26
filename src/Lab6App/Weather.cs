using System;

namespace Lab6App
{
    public struct Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public double Temp { get; set; }
        public string Description { get; set; }

        public override string ToString() =>
            $"{Country}/{Name}: {Temp:F1}°C - {Description}";
    }
}
