using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.Shared.MVVM;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        CalendarInfo calendar;

        public MainPageViewModel()
        {
            Calendar = new CalendarInfo();
        }

        public CalendarInfo Calendar
        {
            get
            {
                return calendar;
            }

            set
            {
                SetProperty(ref calendar, value);
            }
        }
    }
}
