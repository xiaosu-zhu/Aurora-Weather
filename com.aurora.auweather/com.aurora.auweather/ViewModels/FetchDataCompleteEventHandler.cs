namespace Com.Aurora.AuWeather.ViewModels
{
    public delegate void FetchDataCompleteEventHandler(object sender, FetchDataCompleteEventArgs e);

    public class FetchDataCompleteEventArgs
    {
    }

    public delegate void FetchDataFailedEventHandler(object sender, FetchDataFailedEventArgs e);

    public class FetchDataFailedEventArgs
    {
        public string Message { get; private set; }

        public FetchDataFailedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}