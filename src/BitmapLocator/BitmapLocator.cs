using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenScraper
{
    public partial class BitmapLocator : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        private Bitmap _bitmapAtCursor = new Bitmap(300, 300, PixelFormat.Format32bppArgb);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BitmapLocator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Tick event for the timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveTimer_Tick(object sender, EventArgs e)
        {
            var cursor = new Point();
            GetCursorPos(ref cursor);

            UpdateBitmapAtLocation(cursor);
            BackgroundImage = _bitmapAtCursor;
        }

        /// <summary>
        /// Gets the on-screen bitmap image at the given location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        private void UpdateBitmapAtLocation(Point location)
        {
            _bitmapAtCursor.Dispose();
            _bitmapAtCursor = new Bitmap(300,
                                         300,
                                           PixelFormat.Format32bppArgb);

            using (var gfxScreenshot = Graphics.FromImage(_bitmapAtCursor))
            {

                gfxScreenshot.CopyFromScreen(location.X,
                                            location.Y,
                                            0,
                                            0,
                                            new Size(300, 300),
                                            CopyPixelOperation.SourceCopy);
            }
        }

        /// <summary>
        /// Gets the whole primary screen as a bitmap.
        /// </summary>
        /// <remarks>Wrap the return value inside using or dispose manually.</remarks>
        /// <returns>The screen as a bitmap.</returns>
        private Bitmap GetFullScreenBitmap()
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

            //bitmap.Save("Screenshot.png", ImageFormat.Png);

            return bitmap;
        }
    }
}
