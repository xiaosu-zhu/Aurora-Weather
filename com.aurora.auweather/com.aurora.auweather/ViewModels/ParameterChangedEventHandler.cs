namespace Com.Aurora.AuWeather.ViewModels
{
    public delegate void ParameterChangedEventHandler(object sender, ParameterChangedEventArgs e);

    public class ParameterChangedEventArgs
    {
        public object Parameter { get; private set; }

        public ParameterChangedEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }
}