using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        private readonly List<Point> _foundBitmapLocations = new List<Point>();

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

            buttonFind.Enabled = true;
        }

        /// <summary>
        /// Event handler for buttonFind click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFind_Click(object sender, EventArgs e)
        {
            _foundBitmapLocations.Clear();

            var bitmapsToFind = _fileNames.Select(fileName => new Bitmap(fileName)).ToList();

            Cursor = Cursors.WaitCursor;

            using (var screenBitmap = BitmapOperations.GetFullScreenBitmap())
            {
                foreach (var smallBitmap in bitmapsToFind)
                {
                    if (smallBitmap.Height > screenBitmap.Height || smallBitmap.Width > screenBitmap.Width)
                    {
                        MessageBox.Show("One of the bitmaps is larger than the screen - it's not possible to find it.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    _foundBitmapLocations.AddRange(BitmapOperations.SearchBitmap(smallBitmap, screenBitmap, 0));

                    smallBitmap.Dispose();
                }
            }

            Cursor = Cursors.Default;

            if (_foundBitmapLocations.Count > 0)
            {
                var message =
                    new StringBuilder((bitmapsToFind.Count > 1 ? "The bitmaps were" : "The bitmap was") +
                                      " found at the following location(s): ");
                foreach (var point in _foundBitmapLocations)
                {
                    message.Append(String.Format("({0},{1})", point.X, point.Y));
                }
                MessageBox.Show(message.ToString());
            }
            else
            {
                var message =
                    new StringBuilder((bitmapsToFind.Count > 1 ? "The bitmaps were" : "The bitmap was") +
                                      " not found on the screen.");
                MessageBox.Show(message.ToString());
            }
        }
    }
}
