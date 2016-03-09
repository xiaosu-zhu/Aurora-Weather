using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Com.Aurora.Shared.Helpers;
using System.Threading.Tasks;
using System.Globalization;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Effects;
using Windows.Security.Cryptography.Core;
using Com.Aurora.Shared.Helpers.Crypto;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.HeWeather;
using System.Linq;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class GeneralTest
    {
        [TestMethod]
        public async Task BaseTest()
        {
            string url = "http://apis.baidu.com/heweather/pro/weather";
            string[] param = { "city=beijing" };
            var keys = (await FileIOHelper.ReadStringFromAssetsAsync("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
            var actual = await BaiduRequestHelper.RequestWithKeyAsync(url, param, keys[0]);
            string notexpected = null;
            Assert.AreNotEqual(notexpected, actual);
        }

        [TestMethod]
        public void DateTimeParseTest1()
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var actual = DateTime.ParseExact("2016-01-31", "yyyy-MM-dd", provider);
            var expected = new DateTime(2016, 1, 31);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DateTimeParseTest2()
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var actual = DateTime.ParseExact("2016-01-25 13:00", "yyyy-MM-dd HH:mm", provider);
            var expected = new DateTime(2016, 1, 25, 13, 00, 00);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TempratureTest1()
        {
            var actual = Temperature.FromCelsius(25).Fahrenheit;
            var expected = 77;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TempratureTest2()
        {
            var actual = Temperature.FromFahrenheit(59).Celsius;
            var expected = 15;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TempratureTest3()
        {
            var aa = Temperature.FromKelvin(279);
            var actual = aa.Fahrenheit;
            var expected = 43;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ApiRequestTest()
        {
            //string[] param = { "search=allworld" };
            //await FileIOHelper.SaveFile("actual", "cityid.txt");
        }

        [TestMethod]
        public async Task ReadCityTest()
        {
            var str = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
        }

        [TestMethod]
        public void StringStreamText()
        {
            string origin = "mother fucker what you are!\r\t";
            string actual = FileIOHelper.StreamToString(FileIOHelper.StringToStream(origin));
            Assert.AreEqual(origin, actual);
        }

        [TestMethod]
        public void PinYinTest()
        {
            string origin = "苟利国家生死以，岂因祸福避趋之, hello thank you,你好//";
            string actual = PinYinHelper.GetPinyin(origin);
            string expected = "gouliguojiashengsiyi，qiyinhuofubiquzhi, hello thank you,nihao//";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SettingsModelTest1()
        {
            ReadWrtieSettingGroupTestClass origin = new ReadWrtieSettingGroupTestClass(TimeSpan.FromDays(3.045), 12, "nimabi", 2.14, true, DateTime.Now, new DateTime[] { DateTime.Now, new DateTime(1999, 10, 10), new DateTime(2016, 10, 10) }, new string[] { "cao", "ni", "ma" });
            //RoamingSettingsHelper.WriteGroupSettings(origin);
            ReadWrtieSettingGroupTestClass actual;
            //RoamingSettingsHelper.ReadGroupSettings(out actual);
            //Assert.AreEqual(origin, actual);
        }

        [TestMethod]
        public void ThunderPathTest()
        {
            Thunder t = new Thunder(2.5f, new System.Numerics.Vector2(150, 700));
        }

        [TestMethod]
        public void EncryptTest()
        {
            string strMsg = string.Empty;           // Message string
            string strAlgName = string.Empty;       // Algorithm name
            uint keyLength = 0;                   // Length of key
            BinaryStringEncoding encoding = BinaryStringEncoding.Utf8;          // Binary encoding type
            strAlgName = SymmetricAlgorithmNames.AesCbc;
            keyLength = 32;
            CryptographicKey key = CryptoHelper.GenerateKey(strAlgName, keyLength);                   // Symmetric Key
            strMsg = "1234567812345678";


            IBuffer buffEncrypted = CryptoHelper.CipherEncryption(strMsg, strAlgName, encoding, key);
            var actual = CryptoHelper.CipherDecryption(strAlgName, buffEncrypted, encoding, key);
            Assert.AreEqual(strMsg, actual);
        }

        [TestMethod]
        public void LunarCalendarTest()
        {
            CalendarInfo c = new CalendarInfo();
        }

        [TestMethod]
        public async void GeoLocateTest()
        {
            var data = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
            var result = JsonHelper.FromJson<CityIdContract>(data);
            var citys = CityInfo.CreateList(result);
            var final = from m in citys
                        orderby CalcDistance(m, new Location(39.92f, 116.46f)) ascending
                        select m;
            Debug.WriteLine((final.ToArray())[0]);
        }

        private const double EARTH_RADIUS = 6378.137;//地球半径
        private float CalcDistance(CityInfo m, Location location)
        {
            var lat1 = Tools.DegreesToRadians(m.Location.Latitude);
            var lat2 = Tools.DegreesToRadians(location.Latitude);
            var a = lat1 - lat2;
            var b = Tools.DegreesToRadians(m.Location.Longitude) - Tools.DegreesToRadians(location.Longitude);
            var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s *= EARTH_RADIUS;
            return (float)Math.Round(s * 10000) / 10000f;
        }
    }
}
