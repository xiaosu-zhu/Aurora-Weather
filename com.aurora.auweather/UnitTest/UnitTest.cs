using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using com.aurora.shared.Helpers;
using System.Threading.Tasks;
using System.Globalization;
using com.aurora.auweather.Models;

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
            var keys = (await FileIOHelper.ReadStringFromAssets("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
            var actual = await BaiduRequestHelper.RequestWithKey(url, param, keys[0]);
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
            var actual = Temprature.FromCelsius(25).Fahrenheit;
            var expected = 77;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TempratureTest2()
        {
            var actual = Temprature.FromFahrenheit(59).Celsius;
            var expected = 15;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TempratureTest3()
        {
            var aa = Temprature.FromKelvin(279);
            var actual = aa.Fahrenheit;
            var expected = 43;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task ApiRequestTest()
        {
            //string[] param = { "search=allworld" };
            //await FileIOHelper.SaveFile("actual", "cityid.txt");
        }
        [TestMethod]
        public async Task ReadCityTest()
        {
            var str = await FileIOHelper.ReadStringFromAssets("cityid.txt");
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
            SettingsModel origin = new SettingsModel();
            origin.AllowLocation = false;
            origin.RefreshState = RefreshState.none;
            origin.SaveSettings();
            var actual = SettingsModel.ReadSettings();
            Assert.AreEqual(origin, actual);
        }
    }
}
