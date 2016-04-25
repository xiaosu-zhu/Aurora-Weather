// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CrashReportPage : Page
    {
        private UnhandledExceptionEventArgs crash;
        private const string UPLOAD_CRASH = "http://144.168.57.150/upload.php";

        public CrashReportPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            crash = e.Parameter as UnhandledExceptionEventArgs;
        }

        private async void CrashDialog_Loaded(object sender, RoutedEventArgs e)
        {

            CrashInfo.Text = crash.Message + '\n' + "Error Code:" + crash.Exception.HResult.ToHexString();
            CrashDialog.PrimaryButtonClick += async (m, v) =>
            {
                try
                {
                    var crashLOG = await FileIOHelper.AppendLogtoCacheAsync(crash.Exception, Guid.NewGuid().ToString());
                    if (!ReportBox.Text.IsNullorEmpty())
                        await FileIO.AppendTextAsync(crashLOG, "User Voice = " + ReportBox.Text + Environment.NewLine);

                    var fileBytes = await GetBytesAsync(crashLOG);
                    UploadFilesToServer(new Uri(UPLOAD_CRASH), null, crashLOG.Name, "application/octet-stream", fileBytes);
                }
                catch (Exception)
                {
                    CrashDialog.PrimaryButtonText = "Failed";
                }
            };
            CrashDialog.SecondaryButtonClick += (a, q) =>
            {
                Application.Current.Exit();
            };
            await CrashDialog.ShowAsync();
        }

        /// <summary>
        /// Creates HTTP POST request & uploads database to server. Author : Farhan Ghumra
        /// </summary>
        private void UploadFilesToServer(Uri uri, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            string boundary = "----------" + Guid.NewGuid().ToString();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.BeginGetRequestStream((result) =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    string responseString = streamReader.ReadToEnd();
                                    //responseString is depend upon your web service.
                                    if (responseString == "Success")
                                    {
                                        App.Current.Exit();
                                    }
                                    else
                                    {
                                        CrashDialog.PrimaryButtonText = "Failed";
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            CrashDialog.PrimaryButtonText = "Failed";
                        }
                    }, null);
                }
                catch (Exception)
                {
                    CrashDialog.PrimaryButtonText = "Failed";
                }
            }, httpWebRequest);
        }

        /// <summary>
        /// Writes multi part HTTP POST request. Author : Farhan Ghumra
        /// </summary>
        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "–-\r\n");
            /// the form data, properly formatted 
            /// Content-Disposition: form-data; name="text1" 
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            /// Content-Disposition: form-data; name="userfile1"; filename="E:/s" 
            /// Content-Type: application/octet-stream 
            string fileheaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }

        /// <summary>
        /// Writes string to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes byte array to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns byte array from StorageFile. Author : Farhan Ghumra
        /// </summary>
        private async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }
    }
}
