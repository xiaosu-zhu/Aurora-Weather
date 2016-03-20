using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
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

        public static async Task AppendLogtoCacheAsync(string v)
        {
            var cache = ApplicationData.Current.LocalCacheFolder;
            var log = await cache.CreateFileAsync("LOG", CreationCollisionOption.OpenIfExists);
            await FileIO.AppendTextAsync(log, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":  " + v + Environment.NewLine);
        }

        public static async Task<IReadOnlyList<StorageFile>> GetFilesFromAssetsAsync(string path)
        {
            StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assets = await installedLocation.GetFolderAsync("Assets");
            try
            {
                var folder = await assets.GetFolderAsync(path);
                var files = await folder.GetFilesAsync();
                return files;
            }
            catch (Exception)
            {
                return null;
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

        public static async Task<List<KeyValuePair<Uri, string>>> GetThumbnailUrisFromAssetsAsync(string path)
        {
            var files = await GetFilesFromAssetsAsync(path);
            var restul = new List<KeyValuePair<Uri, string>>();
            if (files != null && files.Count > 0)
                foreach (var file in files)
                {
                    if (file.DisplayName.EndsWith("_t"))
                    {
                        restul.Add(new KeyValuePair<Uri, string>
                            (new Uri("ms-appx:///Assets/" + path.Replace("\\", "/") + '/' + file.Name),
                            (file.DisplayName.Remove(file.DisplayName.Length - 2)).Substring(3)));
                    }
                }
            return restul;
        }

        public static async Task<IReadOnlyList<StorageFile>> GetFilesFromLocalAsync(string path)
        {
            var local = ApplicationData.Current.LocalFolder;
            try
            {
                var folder = await local.GetFolderAsync(path);
                return await folder.GetFilesAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static async Task<Uri> GetFileUriFromLocalAsync(string path, string fileName)
        {
            var local = ApplicationData.Current.LocalFolder;
            try
            {
                var folder = await local.GetFolderAsync(path);
                var file = await folder.GetFileAsync(fileName);
                return new Uri("ms-appdata:///local/" + path.Replace("\\", "/") + '/' + file.Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Uri> SaveFiletoLocalAsync(string path, StorageFile file, string desiredName)
        {
            var local = ApplicationData.Current.LocalFolder;
            try
            {
                var folder = await local.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);
                await file.CopyAsync(folder, desiredName, NameCollisionOption.ReplaceExisting);
                return new Uri("ms-appdata:///local/" + path.Replace("\\", "/") + '/' + desiredName);
            }
            catch (Exception)
            {
                return null;
            }
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

        public static async Task<Uri> GetFileUriFromAssetsAsync(string path, int index)
        {
            try
            {
                var files = await GetFilesFromAssetsAsync(path);
                List<StorageFile> result = new List<StorageFile>();
                result.AddRange(files);
                for (int i = result.Count - 1; i > -1; i--)
                {
                    if ((result[i]).DisplayName.EndsWith("_t"))
                    {
                        result.RemoveAt(i);
                    }
                }
                return new Uri("ms-appx:///Assets/" + path.Replace("\\", "/") + '/' + result[index].Name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<StorageFile> GetFilebyUriAsync(Uri uri)
        {
            return await StorageFile.GetFileFromApplicationUriAsync(uri);
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

        public static async Task<IRandomAccessStream> ReadRandomAccessStreamFromAssetsAsync(string fileName)
        {
            var uri = new Uri("ms-appx:///Assets/" + fileName);
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            stream.Seek(0);
            return stream;
        }

        public static async Task<IRandomAccessStream> ReadRandomAccessStreamByUriAsync(Uri uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var stream = await file.OpenAsync(FileAccessMode.Read);
            stream.Seek(0);
            return stream;
        }
    }
}
