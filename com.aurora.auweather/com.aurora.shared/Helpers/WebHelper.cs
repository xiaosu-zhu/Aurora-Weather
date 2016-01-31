using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        public static async Task<string> RequestWithKey(string url, string[] pars, string apikey)
        {
            var strURL = url;
            if (pars != null && pars.Length > 0)
            {
                strURL += '?';
                foreach (var param in pars)
                {
                    strURL += param + '&';
                }
                strURL = strURL.Remove(strURL.Length - 1);
            }
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
        public static async Task<string> RequestIncludeKey(string url, string[] pars, string apikey)
        {
            var strURL = url;
            if (pars != null && pars.Length > 0)
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
}
