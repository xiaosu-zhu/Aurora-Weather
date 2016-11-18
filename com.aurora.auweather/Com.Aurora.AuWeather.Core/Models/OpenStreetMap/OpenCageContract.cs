using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.Models.OpenStreetMap
{
    public class Licenses
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Rate
    {
        public int limit { get; set; }
        public int remaining { get; set; }
        public int reset { get; set; }
    }

    public class DMS
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class Mercator
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class OSM
    {
        public string edit_url { get; set; }
        public string url { get; set; }
    }

    public class Currency
    {
        public IList<string> alternate_symbols { get; set; }
        public string decimal_mark { get; set; }
        public string html_entity { get; set; }
        public string iso_code { get; set; }
        public int iso_numeric { get; set; }
        public string name { get; set; }
        public int smallest_denomination { get; set; }
        public string subunit { get; set; }
        public int subunit_to_unit { get; set; }
        public string symbol { get; set; }
        public int symbol_first { get; set; }
        public string thousands_separator { get; set; }
    }

    public class Rise
    {
        public int apparent { get; set; }
        public int astronomical { get; set; }
        public int civil { get; set; }
        public int nautical { get; set; }
    }

    public class Set
    {
        public int apparent { get; set; }
        public int astronomical { get; set; }
        public int civil { get; set; }
        public int nautical { get; set; }
    }

    public class Sun
    {
        public Rise rise { get; set; }
        public Set set { get; set; }
    }

    public class Timezone
    {
        public string name { get; set; }
        public int now_in_dst { get; set; }
        public int offset_sec { get; set; }
        public int offset_string { get; set; }
        public string short_name { get; set; }
    }

    public class What3words
    {
        public string words { get; set; }
    }

    public class Annotations
    {
        public DMS DMS { get; set; }
        public string MGRS { get; set; }
        public string Maidenhead { get; set; }
        public Mercator Mercator { get; set; }
        public OSM OSM { get; set; }
        public int callingcode { get; set; }
        public Currency currency { get; set; }
        public string geohash { get; set; }
        public double qibla { get; set; }
        public Sun sun { get; set; }
        public Timezone timezone { get; set; }
        public What3words what3words { get; set; }
    }

    public class Components
    {
        public string _type { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string county { get; set; }
        public string region { get; set; }
        public string state { get; set; }
        public string town { get; set; }
    }

    public class Geometry
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Results
    {
        public Annotations annotations { get; set; }
        public Components components { get; set; }
        public int confidence { get; set; }
        public string formatted { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class StayInformed
    {
        public string blog { get; set; }
        public string twitter { get; set; }
    }

    public class Timestamp
    {
        public string created_http { get; set; }
        public int created_unix { get; set; }
    }

    public class OpenCageContract
    {
        public string documentation { get; set; }
        public IList<Licenses> licenses { get; set; }
        public Rate rate { get; set; }
        public IList<Results> results { get; set; }
        public Status status { get; set; }
        public StayInformed stay_informed { get; set; }
        public string thanks { get; set; }
        public Timestamp timestamp { get; set; }
        public int total_results { get; set; }
    }




}
