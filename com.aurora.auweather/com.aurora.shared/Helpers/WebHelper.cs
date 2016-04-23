// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Extensions;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Com.Aurora.Shared.Helpers
{
    public static class BaiduRequestHelper
    {
        /// <summary>
        /// 发送带有 ApiKey 的 HTTP 请求
        /// </summary>
        /// <param name="url">请求的 URL</param>
        /// <param name="pars">请求的参数</param>
        /// <param name="apikey">百度 API Key</param>
        /// <returns>请求结果</returns>
        public static async Task<string> RequestWithKeyAsync(string url, string[] pars, string apikey)
        {
            var strURL = url;
            if (!pars.IsNullorEmpty())
            {
                strURL += '?';
                foreach (var param in pars)
                {
                    strURL += param + '&';
                }
                strURL = strURL.Remove(strURL.Length - 1);
            }
            try
            {
                WebRequest request;
                request = WebRequest.Create(strURL);
                request.Method = "GET";
                // 添加header
                request.Headers["apikey"] = apikey;
                WebResponse response;
                response = await request.GetResponseAsync();
                Stream s;
                s = response.GetResponseStream();
                string StrDate = "";
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate + "\r\n";
                }

                return strValue;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }

    public static class ApiRequestHelper
    {
        /// <summary>
        /// 发送 HTTP 请求
        /// </summary>
        /// <param name="url">请求的 URL</param>
        /// <param name="pars">请求的参数</param>
        /// <param name="apikey">API Key</param>
        /// <returns>请求结果</returns>
        public static async Task<string> RequestIncludeKeyAsync(string url, string[] pars, string apikey)
        {
            var strURL = url;
            if (!pars.IsNullorEmpty())
            {
                strURL += '?';
                foreach (var param in pars)
                {
                    strURL += param + '&';
                }
                strURL += "key=" + apikey;
            }
            WebRequest request;
            request = WebRequest.Create(strURL);
            request.Method = "GET";
            WebResponse response;
            response = await request.GetResponseAsync();
            Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
    }

    public static class CaiyunRequestHelper
    {
        private static readonly string nowUrl = "https://api.caiyunapp.com/v2/";
        public static async Task<string> RequestNowWithKeyAsync(float lon, float lat, string key)
        {
            var strURL = nowUrl + key + '/' + lon.ToString("0.0000") + ',' + lat.ToString("0.0000") + "/realtime.json";
            WebRequest request;
            request = WebRequest.Create(strURL);
            request.Method = "GET";
            WebResponse response;
            response = await request.GetResponseAsync();
            Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

        public static async Task<string> RequestForecastWithKeyAsync(float lon, float lat, string key)
        {
            var strURL = nowUrl + key + '/' + lon.ToString("0.0000") + ',' + lat.ToString("0.0000") + "/forecast.json";
            WebRequest request;
            request = WebRequest.Create(strURL);
            request.Method = "GET";
            WebResponse response;
            response = await request.GetResponseAsync();
            Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
    }

    public static class WundergroundRequestHelper
    {
        private static readonly string url = "http://api.wunderground.com/api/{0}/geolookup/conditions/forecast/hourly/q";

        public static async Task<string> GeoLookup(string key, float lat, float lon)
        {
            var strURL = string.Format(url, key) + '/' + lat.ToString() + ',' + lon.ToString() + ".json";
            WebRequest request;
            request = WebRequest.Create(strURL);
            request.Method = "GET";
            WebResponse response;
            response = await request.GetResponseAsync();
            Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

        public static async Task<string> GetResult(string key, string zmw)
        {
            var strURL = string.Format(url, key) + '/' + zmw + ".json";
            WebRequest request;
            request = WebRequest.Create(strURL);
            request.Method = "GET";
            WebResponse response;
            response = await request.GetResponseAsync();
            Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
    }


    public static class WebHelper
    {
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
