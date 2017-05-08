using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace ScreenScraper
{
    public class TextOperations
    {
        /// <summary>
        /// Parses and returns any text on the right-hand side of the location on the bitmap using Tesseract.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="location"></param>
        public static string GetTextNearLocation(Bitmap bitmap, Point location)
        {
            //Just some sample values for debugging and testing...
            var croppedArea = new Rectangle(new Point(location.X + 40, location.Y), new Size(150, 17));
            using (var croppedBitmap = new Bitmap(croppedArea.Width, croppedArea.Height))
            {
                using (var graphics = Graphics.FromImage(croppedBitmap))
                {
                    graphics.DrawImage(bitmap, -croppedArea.X, -croppedArea.Y);

                    using (var scaledBitmap = BitmapOperations.ResizeBitmap(croppedBitmap, croppedBitmap.Width * 3, croppedBitmap.Height * 3))
                    {
                        using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                        {
                            using (var page = engine.Process(scaledBitmap, PageSegMode.SingleLine))
                            {
                                return page.GetText();
                            }
                        }
                    }
                }
            }
        }
    }
}
