namespace Com.Aurora.AuWeather.ViewModels.Events
{
    public class FetchDataCompleteEventArgs
    {

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