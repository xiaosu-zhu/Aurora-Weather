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
        public static async Task<string> ReadStringFromAssetsAsync(string fileName)
        {
            if (fileName == null)
                throw new ArgumentException();
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + fileName));
            return await FileIO.ReadTextAsync(sFile);
        }

        /// <summary>
        /// 从安装目录读取文件，返回 byte[]
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadAllBytesFromInstallAsync(string fileName)
        {
            var uri = new Uri("ms-appx:///" + fileName);
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var buffer = await FileIO.ReadBufferAsync(file);

            return buffer.ToArray();
        }

        /// <summary>
        /// 将缓冲区写入存储目录（覆盖）
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="buffer">要存储的缓冲区</param>
        public static async Task SaveBuffertoStorageAsync(string fileName, IBuffer buffer)
        {
            if (fileName == null)
                throw new ArgumentException();
            var storeFolder = ApplicationData.Current.LocalFolder;
            var file = await storeFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteBufferAsync(file, buffer);
        }

        /// <summary>
        /// 将文本写入存储目录（覆盖）
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task SaveStringtoStorageAsync(string fileName, string content)
        {
            if (fileName == null)
                throw new ArgumentException();
            var storeFolder = ApplicationData.Current.LocalFolder;
            var file = await storeFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, content);
        }

        /// <summary>
        /// 从存储目录读取文本
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<IBuffer> ReadBufferFromStorageAsync(string fileName)
        {
            if (fileName == null)
                throw new ArgumentException();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(fileName);

            return await FileIO.ReadBufferAsync(file);
        }

        public static async Task<string> ReadStringFromStorageAsync(string fileName)
        {
            if (fileName == null)
                throw new ArgumentException();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(fileName);
            return await FileIO.ReadTextAsync(file);
        }

        /// <summary>
        /// 从Assets读取Buffer
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<IBuffer> ReadBufferFromAssetsAsync(string fileName)
        {
            if (fileName == null)
                throw new ArgumentException();
            StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + fileName));
            return await FileIO.ReadBufferAsync(sFile);
        }
    }
}
