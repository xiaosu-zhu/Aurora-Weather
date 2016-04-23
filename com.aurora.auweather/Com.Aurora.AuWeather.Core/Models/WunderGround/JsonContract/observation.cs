using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class observation
    {
        [DataMember]
        public image image;
        [DataMember]
        public display_location display_location;
        [DataMember]
        public observation_location observation_location;
        [DataMember]
        public estimated estimated;
        [DataMember]
        public string station_id;
        [DataMember]
        public string observation_time;
        [DataMember]
        public string observation_time_rfc822;
        [DataMember]
        public string observation_epoch;
        [DataMember]
        public string local_time_rfc822;
        [DataMember]
        public string local_epoch;
        [DataMember]
        public string local_tz_short;
        [DataMember]
        public string local_tz_long;
        [DataMember]
        public string local_tz_offset;
        [DataMember]
        public string weather;
        [DataMember]
        public string temperature_string;
        [DataMember]
        public float temp_f;
        [DataMember]
        public float temp_c;
        [DataMember]
        public string relative_humidity;
        [DataMember]
        public string wind_string;
        [DataMember]
        public string wind_dir;
        [DataMember]
        public float wind_degrees;
        [DataMember]
        public float wind_mph;
        [DataMember]
        public float wind_gust_mph;
        [DataMember]
        public float wind_kph;
        [DataMember]
        public float wind_gust_kph;
        [DataMember]
        public string pressure_mb;
        [DataMember]
        public string pressure_in;
        [DataMember]
        public string pressure_trend;
        [DataMember]
        public string dewpoint_string;
        [DataMember]
        public float dewpoint_f;
        [DataMember]
        public float dewpoint_c;
        [DataMember]
        public string heat_index_string;
        [DataMember]
        public string heat_index_f;
        [DataMember]
        public string heat_index_c;
        [DataMember]
        public string windchill_string;
        [DataMember]
        public string windchill_f;
        [DataMember]
        public string windchill_c;
        [DataMember]
        public string feelslike_string;
        [DataMember]
        public string feelslike_f;
        [DataMember]
        public string feelslike_c;
        [DataMember]
        public string visibility_mi;
        [DataMember]
        public string visibility_km;
        [DataMember]
        public string solarradiation;
        [DataMember]
        public string UV;
        [DataMember]
        public string precip_1hr_string;
        [DataMember]
        public string precip_1hr_in;
        [DataMember]
        public string precip_1hr_metric;
        [DataMember]
        public string precip_today_string;
        [DataMember]
        public string precip_today_in;
        [DataMember]
        public string precip_today_metric;
        [DataMember]
        public string icon;
        [DataMember]
        public string icon_url;
        [DataMember]
        public string forecast_url;
        [DataMember]
        public string history_url;
        [DataMember]
        public string ob_url;
        [DataMember]
        public string nowcast;
    }

    [DataContract]
    public class estimated
    {
    }

    [DataContract]
    public class observation_location
    {
        [DataMember]
        public string full;
        [DataMember]
        public string city;
        [DataMember]
        public string state;
        [DataMember]
        public string country;
        [DataMember]
        public string country_iso3166;
        [DataMember]
        public string latitude;
        [DataMember]
        public string longitude;
        [DataMember]
        public string elevation;
    }

    [DataContract]
    public class display_location
    {
        [DataMember]
        public string full;
        [DataMember]
        public string city;
        [DataMember]
        public string state;
        [DataMember]
        public string state_name;
        [DataMember]
        public string country;
        [DataMember]
        public string country_iso3166;
        [DataMember]
        public string zip;
        [DataMember]
        public string magic;
        [DataMember]
        public string wmo;
        [DataMember]
        public string latitude;
        [DataMember]
        public string longitude;
        [DataMember]
        public string elevation;
    }

    [DataContract]
    public class image
    {
        [DataMember]
        public string url;
        [DataMember]
        public string title;
        [DataMember]
        public string link;
    }
}
