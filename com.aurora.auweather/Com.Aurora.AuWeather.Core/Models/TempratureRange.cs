namespace Com.Aurora.AuWeather.Models
{
    internal class TempratureRange
    {
        public Temperature Low { get; set; }
        public Temperature High { get; set; }

        public TempratureRange(Temperature low, Temperature high)
        {
            Low = low;
            High = high;
        }

        public static TempratureRange FromCelsius(int low, int high)
        {
            var Low = Temperature.FromCelsius(low);
            var High = Temperature.FromCelsius(high);
            return new TempratureRange(Low, High);
        }
        public static TempratureRange FromFahrenheit(int low, int high)
        {
            var Low = Temperature.FromFahrenheit(low);
            var High = Temperature.FromFahrenheit(high);
            return new TempratureRange(Low, High);
        }
        public static TempratureRange FromKelvin(int low, int high)
        {
            var Low = Temperature.FromKelvin(low);
            var High = Temperature.FromKelvin(high);
            return new TempratureRange(Low, High);
        }
    }
}
