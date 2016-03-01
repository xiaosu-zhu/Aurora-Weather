namespace Com.Aurora.AuWeather.ViewModels.Events
{
    public class ParameterChangedEventArgs
    {
        public object Parameter { get; private set; }

        public ParameterChangedEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }
}