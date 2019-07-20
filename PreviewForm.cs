using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FastNeuralColor
{
    public partial class PreviewForm : Form
    {
        public PreviewForm( Bitmap inimage, Bitmap outimage )
        {
            InitializeComponent();

            pictureBox1.Image = inimage;
            pictureBox2.Image = outimage;
        }

        private void btnSave_Click( object sender, EventArgs e )
        {
            var filters = new Dictionary<ImageFormat, string>
            {
                { ImageFormat.Bmp, "*.bmp" },
                { ImageFormat.Emf, "*.emf" },
                { ImageFormat.Exif, "*.exif" },
                { ImageFormat.Gif, "*.gif" },
                { ImageFormat.Icon, "*.ico" },
                { ImageFormat.Jpeg, "*.jpg" },
                { ImageFormat.Png, "*.png" },
                { ImageFormat.Tiff, "*.tiff" },
                { ImageFormat.Wmf, "*.wmf" }
            };

            var dlgSaveImage = new SaveFileDialog
            {
                InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ),
                Title = "Сохранить изображение",
                Filter = string.Join( "|", filters.Values.Select( x => $"Изображения ({x})|{x}" ) )
            };

            if ( dlgSaveImage.ShowDialog() != DialogResult.OK )
                return;

            try
            {
                var fmtset = new[] { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Exif };

                var fmt = dlgSaveImage.FilterIndex > fmtset.Length ? ImageFormat.Jpeg : fmtset[ dlgSaveImage.FilterIndex - 1 ];

                pictureBox2.Image.Save( dlgSaveImage.FileName, fmt );
            }
            catch ( Exception ex )
            {
                MessageBox.Show( "Не удалось сохранить изображение: " + ex.Message, "Ошибка!" );
            }
        }
    }
}
