using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Com.Aurora.Shared.Helpers
{
    public class FileIOHelper
    {
        /// <summary>
        /// Convert UTF-8 encoding stream to string, sync
        /// </summary>
        /// <param name="stream">source</param>
        /// <returns></returns>
        public static string StreamToString(Stream src)
        {
            src.Position = 0;
            using (StreamReader reader = new StreamReader(src, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Convert UTF-8 encoding string to stream, sync
        /// </summary>
        /// <param name="src">source</param>
        /// <returns></returns>
        public static Stream StringToStream(string src)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(src);
            return new MemoryStream(byteArray);
        }

        /// <summary>
        /// Find file from localfolder, and read it
        /// </summary>
        /// <param name="fileName">file total name</param>
        /// <returns></returns>
        public static async Task<string> ReadStringFromAssets(string fileName)
        {
            if (fileName == null)
                throw new ArgumentException();
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/"+fileName));
            return await FileIO.ReadTextAsync(sFile);
        }

        public static async Task SaveFile(string sLine, string fileName)
        {
            StorageFolder cacheFolder = ApplicationData.Current.LocalFolder;
            StorageFile cacheFile = await cacheFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var flow = await cacheFile.OpenAsync(FileAccessMode.ReadWrite);
            using (var outputStream = flow.GetOutputStreamAt(0))
            {
                using (var dataWriter = new DataWriter(outputStream))
                {
                    dataWriter.WriteString(sLine);
                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }
            flow.Dispose();
        }

        /// <summary>
        /// 从安装目录读取文件，返回 byte[]
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadAllBytes(string filename)
        {
            var uri = new Uri("ms-appx:///" + filename);
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var buffer = await FileIO.ReadBufferAsync(file);

            return buffer.ToArray();
        }
    }
}
