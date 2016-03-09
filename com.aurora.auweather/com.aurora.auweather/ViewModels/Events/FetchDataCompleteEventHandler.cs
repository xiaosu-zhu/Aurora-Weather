namespace Com.Aurora.AuWeather.ViewModels.Events
{
    public class FetchDataCompleteEventArgs
    {
        public object Parameter { get; private set; }

        public FetchDataCompleteEventArgs()
        {

        }

        public FetchDataCompleteEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }

    public class FetchDataFailedEventArgs
    {
        public string Message { get; private set; }

        public FetchDataFailedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}