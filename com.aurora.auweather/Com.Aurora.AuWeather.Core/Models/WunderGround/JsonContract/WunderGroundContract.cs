using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class WunderGroundContract
    {
        [DataMember]
        public response response;
        [DataMember]
        public location location;
        [DataMember]
        public observation current_observation;
        [DataMember]
        public forecast forecast;
        [DataMember]
        public hourly[] hourly_forecast;

        internal static WunderGroundContract Generate(string resstr)
        {
            if (resstr.IsNullorEmpty())
            {
                return null;
            }
            return JsonHelper.FromJson<WunderGroundContract>(resstr);
        }
    }
}
