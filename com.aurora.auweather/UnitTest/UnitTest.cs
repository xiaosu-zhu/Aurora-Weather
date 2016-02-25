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
        public void ApiRequestTest()
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
            IBuffer iv;                             // Initialization vector
            CryptographicKey key = CryptoHelper.GenerateKey(strAlgName, keyLength);                   // Symmetric Key
            strMsg = "1234567812345678";


            IBuffer buffEncrypted = CryptoHelper.CipherEncryption(strMsg, strAlgName, encoding, key);
            var actual = CryptoHelper.CipherDecryption(strAlgName, buffEncrypted, encoding, key);
            Assert.AreEqual(strMsg, actual);
        }
    }
}
