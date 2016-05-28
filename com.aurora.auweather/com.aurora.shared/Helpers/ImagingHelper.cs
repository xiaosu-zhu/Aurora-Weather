using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace Com.Aurora.Shared.Helpers
{
    public class ImagingHelper
    {
        public static async Task<BitmapImage> ResizedImage(StorageFile ImageFile, float ratio)
        {
            IRandomAccessStream inputstream = await ImageFile.OpenReadAsync();
            BitmapImage sourceImage = new BitmapImage();
            sourceImage.SetSource(inputstream);

            var origHeight = sourceImage.PixelHeight;
            var origWidth = sourceImage.PixelWidth;
            var newHeight = (int)(origHeight * ratio);
            var newWidth = (int)(origWidth * ratio);

            sourceImage.DecodePixelWidth = newWidth;
            sourceImage.DecodePixelHeight = newHeight;

            return sourceImage;
        }

        public static async Task<WriteableBitmap> GetPictureAsync(StorageFile ImageFile)
        {

            using (IRandomAccessStream stream = await ImageFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                WriteableBitmap bmp = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                await bmp.SetSourceAsync(stream);
                return bmp;
            }
        }

        public static async Task SaveBitmapToFileAsync(WriteableBitmap image, string userId)
        {
            StorageFolder pictureFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePictures", CreationCollisionOption.OpenIfExists);
            var file = await pictureFolder.CreateFileAsync(userId + ".jpg", CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream.AsRandomAccessStream());
                var pixelStream = image.PixelBuffer.AsStream();
                byte[] pixels = new byte[image.PixelBuffer.Length];

                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)image.PixelWidth, (uint)image.PixelHeight, 96, 96, pixels);

                await encoder.FlushAsync();
            }
        }

        public static async Task CropandScaleAsync(StorageFile source, StorageFile dest, Point startPoint, Size size, double m_scaleFactor)
        {
            uint startPointX = (uint)Math.Floor(startPoint.X);
            uint startPointY = (uint)Math.Floor(startPoint.Y);
            uint height = (uint)Math.Floor(size.Height);
            uint width = (uint)Math.Floor(size.Width);
            using (IRandomAccessStream sourceStream = await source.OpenReadAsync(),
                destStream = await dest.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
                var m_displayHeightNonScaled = decoder.OrientedPixelHeight;
                var m_displayWidthNonScaled = decoder.OrientedPixelWidth;

                // Use the native (no orientation applied) image dimensions because we want to handle
                // orientation ourselves.
                BitmapTransform transform = new BitmapTransform();
                BitmapBounds bounds = new BitmapBounds();

                bounds.X = (uint)(startPointX * m_scaleFactor);
                bounds.Y = (uint)(startPointY * m_scaleFactor);
                bounds.Height = (uint)(height * m_scaleFactor);
                bounds.Width = (uint)(width * m_scaleFactor);
                transform.Bounds = bounds;

                // Scaling occurs before flip/rotation, therefore use the original dimensions
                // (no orientation applied) as parameters for scaling.
                transform.ScaledHeight = (uint)(decoder.PixelHeight * m_scaleFactor);
                transform.ScaledWidth = (uint)(decoder.PixelWidth * m_scaleFactor);
                transform.Rotation = BitmapRotation.None;

                // Fant is a relatively high quality interpolation mode.
                transform.InterpolationMode = BitmapInterpolationMode.Fant;
                BitmapPixelFormat format = decoder.BitmapPixelFormat;
                BitmapAlphaMode alpha = decoder.BitmapAlphaMode;

                // Set the encoder's destination to the temporary, in-memory stream.
                PixelDataProvider pixelProvider = await decoder.GetPixelDataAsync(
                        format,
                        alpha,
                        transform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.ColorManageToSRgb
                        );

                byte[] pixels = pixelProvider.DetachPixelData();


                Guid encoderID = Guid.Empty;

                switch (dest.FileType.ToLower())
                {
                    case ".png":
                        encoderID = BitmapEncoder.PngEncoderId;
                        break;
                    case ".bmp":
                        encoderID = BitmapEncoder.BmpEncoderId;
                        break;
                    default:
                        encoderID = BitmapEncoder.JpegEncoderId;
                        break;
                }

                // Write the pixel data onto the encoder. Note that we can't simply use the
                // BitmapTransform.ScaledWidth and ScaledHeight members as the user may have
                // requested a rotation (which is applied after scaling).
                var encoder = await BitmapEncoder.CreateAsync(encoderID, destStream);

                encoder.SetPixelData(
                    format,
                    alpha,
                    (bounds.Width),
                    (bounds.Height),
                    decoder.DpiX,
                    decoder.DpiY,
                    pixels
                    );

                await encoder.FlushAsync();
            }
        }
        public static async Task<bool> SetWallpaperAsync(StorageFile localAppDataFileName)
        {
            bool success = false;
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
                success = await profileSettings.TrySetLockScreenImageAsync(localAppDataFileName);
            }
            return success;
        }
    }
}
