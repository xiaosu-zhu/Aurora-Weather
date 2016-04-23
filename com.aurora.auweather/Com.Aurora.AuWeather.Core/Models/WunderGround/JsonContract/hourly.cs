using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class hourly
    {
        [DataMember]
        public FCTTIME FCTTIME;
        [DataMember]
        public unit temp;
        [DataMember]
        public unit dewpoint;
        [DataMember]
        public string condition;
        [DataMember]
        public string icon;
        [DataMember]
        public string icon_url;
        [DataMember]
        public string fctcode;
        [DataMember]
        public string sky;
        [DataMember]
        public unit wspd;
        [DataMember]
        public wdir wdir;
        [DataMember]
        public string wx;
        [DataMember]
        public string uvi;
        [DataMember]
        public string humidity;
        [DataMember]
        public unit windchill;
        [DataMember]
        public unit heatindex;
        [DataMember]
        public unit feelslike;
        [DataMember]
        public unit qpf;
        [DataMember]
        public unit snow;
        [DataMember]
        public string pop;
        [DataMember]
        public unit mslp;
    }

    [DataContract]
    public class wdir
    {
        [DataMember]
        public string dir;
        [DataMember]
        public string degrees;
    }

    [DataContract]
    public class unit
    {
        [DataMember]
        public string english;
        [DataMember]
        public string metric;
    }

    [DataContract]
    public class FCTTIME
    {
        [DataMember]
        public string hour;
        [DataMember]
        public string hour_padded;
        [DataMember]
        public string min;
        [DataMember]
        public string min_unpadded;
        [DataMember]
        public string sec;
        [DataMember]
        public string year;
        [DataMember]
        public string mon;
        [DataMember]
        public string mon_padded;
        [DataMember]
        public string mon_abbrev;
        [DataMember]
        public string mday;
        [DataMember]
        public string mday_padded;
        [DataMember]
        public string yday;
        [DataMember]
        public string isdst;
        [DataMember]
        public string epoch;
        [DataMember]
        public string pretty;
        [DataMember]
        public string civil;
        [DataMember]
        public string month_name;
        [DataMember]
        public string month_name_abbrev;
        [DataMember]
        public string weekday_name;
        [DataMember]
        public string weekday_name_night;
        [DataMember]
        public string weekday_name_abbrev;
        [DataMember]
        public string weekday_name_unlang;
        [DataMember]
        public string weekday_name_night_unlang;
        [DataMember]
        public string ampm;
        [DataMember]
        public string tz;
        [DataMember]
        public string age;
        [DataMember]
        public string UTCDATE;
    }
}