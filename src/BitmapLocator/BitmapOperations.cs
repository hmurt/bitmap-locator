using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenScraper
{
    public class BitmapOperations
    {
        /// <summary>
        /// Gets the whole primary screen as a bitmap.
        /// </summary>
        /// <remarks>Wrap the return value inside using or dispose manually.</remarks>
        /// <returns>The screen as a bitmap.</returns>
        public static Bitmap GetFullScreenBitmap()
        {
            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                    Screen.PrimaryScreen.Bounds.Height,
                                    PixelFormat.Format32bppArgb);

            using (var gfxScreenshot = Graphics.FromImage(bitmap))
            {

                gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                             Screen.PrimaryScreen.Bounds.Y,
                                             0,
                                             0,
                                             Screen.PrimaryScreen.Bounds.Size,
                                             CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="bitmap">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(bitmap, destRect, 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Searches the location of a smaller bitmap inside a bigger bitmap.
        /// 
        /// License notice: This source code was modified and optimized from the example by verence333 at
        /// https://www.codeproject.com/Articles/38619/Finding-a-Bitmap-contained-inside-another-Bitmap,
        /// licensed under the CPOL license (https://www.codeproject.com/info/cpol10.aspx).
        /// </summary>
        /// <param name="smallBmp">The bitmap to search for.</param>
        /// <param name="bigBmp">The bitmap where the small bitmap is searched for.</param>
        /// <param name="tolerance">Tolerance value, set this to a number between 0 (exact pixel-by-pixel match) and 0.2 (fuzzy match)</param>
        /// <returns>The location of the small bitmap inside the big bitmap as a Rectangle object.</returns>
        public static IEnumerable<Point> SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
        {
            var smallData =
              smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format24bppRgb);
            var bigData =
              bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format24bppRgb);

            var smallStride = smallData.Stride;
            var bigStride = bigData.Stride;

            var bigWidth = bigBmp.Width;
            var bigHeight = bigBmp.Height - smallBmp.Height + 1;
            var smallWidth = smallBmp.Width * 3;
            var smallHeight = smallBmp.Height;

            var locations = new List<Point>();
            var margin = Convert.ToInt32(255.0 * tolerance);

            unsafe
            {
                var pSmall = (byte*)(void*)smallData.Scan0;
                var pBig = (byte*)(void*)bigData.Scan0;

                var smallOffset = smallStride - smallBmp.Width * 3;
                var bigOffset = bigStride - bigBmp.Width * 3;

                var matchFound = true;

                for (var y = 0; y < bigHeight; y++)
                {
                    for (var x = 0; x < bigWidth; x++)
                    {
                        var pBigBackup = pBig;
                        var pSmallBackup = pSmall;

                        //Look for the small picture.
                        for (var i = 0; i < smallHeight; i++)
                        {
                            int j;
                            matchFound = true;
                            for (j = 0; j < smallWidth; j++)
                            {
                                //With tolerance: pSmall value should be between margins.
                                var inf = pBig[0] - margin;
                                var sup = pBig[0] + margin;
                                if (sup < pSmall[0] || inf > pSmall[0])
                                {
                                    matchFound = false;
                                    break;
                                }

                                pBig++;
                                pSmall++;
                            }

                            if (!matchFound) break;

                            //We restore the pointers.
                            pSmall = pSmallBackup;
                            pBig = pBigBackup;

                            //Next rows of the small and big pictures.
                            pSmall += smallStride * (1 + i);
                            pBig += bigStride * (1 + i);
                        }

                        //If match found, we add the found point to the list.
                        if (matchFound)
                        {
                            locations.Add(new Point(x, y));
                        }
                        //If no match found, we restore the pointers and continue.
                        pBig = pBigBackup;
                        pSmall = pSmallBackup;
                        pBig += 3;
                    }

                    if (matchFound) break;

                    pBig += bigOffset;
                }
            }

            bigBmp.UnlockBits(bigData);
            smallBmp.UnlockBits(smallData);

            return locations;
        }
    }
}
