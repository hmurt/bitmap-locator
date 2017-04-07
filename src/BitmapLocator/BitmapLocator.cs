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

        private List<string> _fileNames = new List<string>();

        private List<Point> _foundBitmapLocations = new List<Point>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BitmapLocator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for buttonBrowse click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            bitmapOpenFileDialog.Filter = "BMP and PNG files (*.bmp, *.png)|*.bmp;*.png";
            if (bitmapOpenFileDialog.ShowDialog() != DialogResult.OK)
                return;

            if (bitmapOpenFileDialog.FileNames.Length > 3)
            {
                MessageBox.Show("Please select 1-3 bitmap files.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _fileNames = bitmapOpenFileDialog.FileNames.ToList();
            textBoxBitmap.Text = String.Join(", ", _fileNames);
        }

        /// <summary>
        /// Event handler for buttonFind click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFind_Click(object sender, EventArgs e)
        {
            var bitmapsToFind = _fileNames.Select(fileName => new Bitmap(fileName)).ToList();

            Cursor = Cursors.WaitCursor;

            using (var screenBitmap = GetFullScreenBitmap())
            {
                foreach (var smallBitmap in bitmapsToFind)
                {
                    if (smallBitmap.Height > screenBitmap.Height || smallBitmap.Width > screenBitmap.Width)
                    {
                        MessageBox.Show("One of the bitmaps is larger than the screen - it's not possible to find it.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    var location = SearchBitmap(smallBitmap, screenBitmap, 0);

                    if (location.Width > 0)
                    {
                        _foundBitmapLocations.Add(new Point(location.X, location.Y));
                    }

                    smallBitmap.Dispose();
                }
            }

            Cursor = Cursors.Default;

            if (_foundBitmapLocations.Count > 0)
            {
                var message = new StringBuilder();
                foreach (var point in _foundBitmapLocations)
                {
                    message.Append(String.Format("({0},{1})", point.X, point.Y));
                }
                MessageBox.Show(String.Format("The bitmap was found at the following location(s): {0}", message));
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

            return bitmap;
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
        /// <returns></returns>
        private Rectangle SearchBitmap(Bitmap smallBmp, Bitmap bigBmp, double tolerance)
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

            var location = Rectangle.Empty;
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

                        //If match found, we return.
                        if (matchFound)
                        {
                            location.X = x;
                            location.Y = y;
                            location.Width = smallBmp.Width;
                            location.Height = smallBmp.Height;
                            break;
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

            return location;
        }
    }
}
