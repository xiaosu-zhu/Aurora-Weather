using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Com.Aurora.Shared.Extensions;
using SQLite;
using Windows.Storage;

namespace Com.Aurora.AuWeather.Core.SQL
{
    public class City
    {
        [PrimaryKey]
        public string Id { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string CityEn { get; set; }
        public string CityZh { get; set; }
        public string CountryCode { get; set; }
        public string CountryEn { get; set; }
        public string ProvinceEn { get; set; }
        public string ProvinceZh { get; set; }

        public string LeaderEn { get; set; }
        public string LeaderZh { get; set; }

        public override string ToString()
        {
            return CountryCode.Equals("CN", StringComparison.OrdinalIgnoreCase) ? (LeaderZh.IsNullorEmpty() ? CityZh : (LeaderZh + " - " + CityZh)) : CityEn;
        }
    }

    public class ChinaCity
    {
        [PrimaryKey]
        public string Id { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string CityEn { get; set; }
        public string CityZh { get; set; }
        public string CountryCode { get; set; }
        public string CountryEn { get; set; }
        public string CountryZh { get; set; }
        public string ProvinceEn { get; set; }
        public string ProvinceZh { get; set; }
        public string LeaderEn { get; set; }
        public string LeaderZh { get; set; }
    }

    public class ForeignCity
    {
        [PrimaryKey]
        public string Id { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string CityEn { get; set; }
        public string CityZh { get; set; }
        public string CountryCode { get; set; }
        public string Continent { get; set; }
        public string CountryEn { get; set; }
    }


    public class SQLAction
    {
        private static readonly string DBPath = Path.Combine(
    ApplicationData.Current.LocalFolder.Path, "db.sqlite");

        public static async Task CheckDB()
        {
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assetsFolder = await appInstalledFolder.GetFolderAsync("Assets");
            var dbFile = await assetsFolder.GetFileAsync("db.sqlite");
            try
            {
                var locFile = await ApplicationData.Current.LocalFolder.GetFileAsync("db.sqlite");
                var proper = await locFile.GetBasicPropertiesAsync();
                if (proper.Size < 1024)
                    await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, "db.sqlite", NameCollisionOption.ReplaceExisting);
            }
            catch (Exception)
            {
                await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, "db.sqlite", NameCollisionOption.ReplaceExisting);
            }
        }

        public static List<City> Query(string query, params object[] args)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                return db.Query<City>(query, args);
            }
        }

        public static List<City> GetAll()
        {
            return Query("SELECT * FROM City");
        }

        public static City Find(Expression<Func<City, bool>> p)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                return db.Find(p);
            }
        }

        public static City Find(string key)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                return db.Find<City>(key);
            }
        }

        public static List<City> FindAll(Expression<Func<City, bool>> p)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                var query = db.Table<City>().Where(p);
                return query.ToList();
            }
        }

        internal static List<City> Sort(Expression<Func<City, float>> p)

        {
            using (var db = new SQLiteConnection(DBPath))
            {
                var query = db.Table<City>().OrderBy(p);
                return query.ToList();
            }
        }
    }
}
