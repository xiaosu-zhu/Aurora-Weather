using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.ViewModels.Events
{
    public class TimeUpdatedEventArgs
    {
        public bool IsDayNightChanged { get; private set; }

        public TimeUpdatedEventArgs(bool b)
        {
            IsDayNightChanged = b;
        }
    }
}
