using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace FlexiWallUI.Utilities
{
    /// <summary>
    /// Functions to handle Bitmaps
    /// </summary>
    public class BitmapUtil
    {
        /// <summary>
        /// Load an Image as BitmapImage
        /// </summary>
        /// <param name="uri"> path to file </param>
        /// <returns></returns>
        public static BitmapImage LoadImage(string uri)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri("pack://application:,,,/" + uri, UriKind.Absolute);
            bmp.EndInit();
            return bmp;
        }

        /// <summary>
        /// convert a Bitmap to a BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage ConvertBitmap(Bitmap bitmap)
        {
            BitmapImage bitmapImage = null;

            if (bitmap != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
