using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastNeuralColor
{
    public partial class MainForm : Form
    {

        #region Private fields

        private const uint OriginalModelSize = 6679424;

        private Bitmap _inputImage;
        private Bitmap _outputImage;
        
        #endregion

        public MainForm()
        {
            InitializeComponent();

            ModelData = new Data( Decompress( new MemoryStream( Properties.Resources.FastNeuralColor ) ) );
        }

        private static MemoryStream Decompress( Stream source )
        {
            var target = new MemoryStream();

            using ( var decompressionStream = new GZipStream( source, CompressionMode.Decompress ) )
            {
                decompressionStream.CopyTo( target );
            }

            target.Seek( 0, SeekOrigin.Begin );

            return target;
        }

        private unsafe void OpenImage( object sender, EventArgs e )
        {
            var dlgOpenImage = new OpenFileDialog()
            {
                InitialDirectory = Path.GetDirectoryName( Application.ExecutablePath ),
                Title = "Выберите изображение для окрашивания",
                Filter =
                    "Изображения (*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf)|*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf|Все файлы(*.*)|*.*",
            };

            if ( dlgOpenImage.ShowDialog() != DialogResult.OK ) return;

            _inputImage = new Bitmap( dlgOpenImage.FileName );

            // Загрузка и преобразование.
            // Lock the bitmap's bits.
            var bmpData = _inputImage.LockBits( new Rectangle( 0, 0, _inputImage.Width, _inputImage.Height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            var ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            var length = bmpData.Stride * _inputImage.Height;

            var rgbValues = new byte[ length ];

            // Copy the RGB values into the array.
            Marshal.Copy( ptr, rgbValues, 0, length );

            // Unlock the bits.
            _inputImage.UnlockBits( bmpData );

            var byteLen = Image.GetPixelFormatSize( bmpData.PixelFormat ) / 8;
            var height = _inputImage.Height;
            var width = _inputImage.Width;

            var matrix = new byte[ height, width ];

            for ( var y = 0; y < height; y++ )
            {
                // Since Width property specifies number of pixels in single scan line, 
                // Stride property contains number of bytes in a scan line. As scan line 
                // may be aligned on four-bytes boundary, stride may differ from bitmap 
                // data width multiplied on number of bytes per pixel. That's why you 
                // should use this property to move to the same position on the next row 
                // instead of any other methods.                        

                var strideY = bmpData.Stride * y;

                for ( var x = 0; x < width; x++ )
                {
                    var index = strideY + x * byteLen;

                    // B
                    var B = 1d * rgbValues[ index++ ];

                    // G
                    var G = 1d * rgbValues[ index++ ];

                    // R
                    var R = 1d * rgbValues[ index++ ];

                    matrix[ y, x ] = ( byte ) Math.Round( ( R + G + B ) / 3.0d );
                }
            }

            // Вывод.
            // 24 bits per pixel.
            const int pixelSize = 3;

            var bmp = new Bitmap( width, height, PixelFormat.Format24bppRgb );

            bmpData = null;

            try
            {
                bmpData = bmp.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );

                for ( var y = 0; y < height; ++y )
                {
                    var targetRow = ( byte* ) bmpData.Scan0 + y * bmpData.Stride;

                    for ( var x = 0; x < width; ++x )
                    {
                        var c = matrix[ y, x ];

                        targetRow[ x * pixelSize + 0 ] = c;
                        targetRow[ x * pixelSize + 1 ] = c;
                        targetRow[ x * pixelSize + 2 ] = c;
                    }
                }
            }
            finally
            {
                if ( bmpData != null ) bmp.UnlockBits( bmpData );
            }

            MainPictureBox.Image = bmp;

            StartStopToolStripMenuItem.Enabled = true;
            SaveImageToolStripMenuItem.Enabled = false;
        }

        private void SaveImage( object sender, EventArgs e )
        {
            new PreviewForm( _inputImage, _outputImage ).ShowDialog();
        }

        private void Close( object sender, EventArgs e )
        {
            if ( ColorizationThread != null && ColorizationThread.IsAlive )
            {
                ColorizationThread.Abort();
            }

            Close();
        }

        private void CloseSender( object sender, CancelEventArgs e )
        {
            if ( ColorizationThread != null && ColorizationThread.IsAlive )
            {
                ColorizationThread.Abort();
            }
        }

        private void StopColorization( object sender, EventArgs e )
        {
            ColorizationThread.Abort();

            StartStopToolStripMenuItem.Click -= StopColorization;
            StartStopToolStripMenuItem.Click += StartColorization;

            StartStopToolStripMenuItem.Text = "Старт";
            OpenImageToolStripMenuItem.Enabled = true;

            MainProgressBar.Value = 0;

            Text = "PABCSoft - FastNeuralColor: Прервано";
        }

        private unsafe void Colorize()
        {
            var timer = DateTime.Now;

            Text = "PABCSoft - FastNeuralColor: Обрабатывается";

            StartStopToolStripMenuItem.Text = "Стоп";

            OpenImageToolStripMenuItem.Enabled = false;
            SaveImageToolStripMenuItem.Enabled = false;

            StartStopToolStripMenuItem.Click -= StartColorization;
            StartStopToolStripMenuItem.Click += StopColorization;

            MainProgressBar.Visible = true;

            var inp = new Bitmap( 720, 560 );
            var tmp = new Bitmap( _inputImage, 640, 480 );

            Graphics.FromImage( inp ).DrawImage( Methods.RestoreColors( tmp ), 40, 40 );

            var BD = inp.LockBits( new Rectangle( 0, 0, 720, 560 ), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );
            var Rand = new Random();

            for ( int y = 0; y < 40; y++ )
            {
                var Addr = BD.Scan0.ToInt64() + y * BD.Stride;

                for ( int x = 0; x < 720; x++ )
                {
                    var L = Rand.Next( 0, 255 );
                    *( Byte* ) ( Addr + x * 3 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 1 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 2 ) = ( Byte ) L;
                }
            }

            for ( int y = 520; y < 560; y++ )
            {
                var Addr = BD.Scan0.ToInt64() + y * BD.Stride;

                for ( int x = 0; x < 720; x++ )
                {
                    var L = Rand.Next( 0, 255 );
                    *( Byte* ) ( Addr + x * 3 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 1 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 2 ) = ( Byte ) L;
                }
            }

            for ( int y = 0; y < 560; y++ )
            {
                var Addr = BD.Scan0.ToInt64() + y * BD.Stride;
                for ( int x = 680; x < 720; x++ )
                {
                    var L = Rand.Next( 0, 255 );
                    *( Byte* ) ( Addr + x * 3 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 1 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 2 ) = ( Byte ) L;
                }
            }
            for ( int y = 0; y < 560; y++ )
            {
                var Addr = BD.Scan0.ToInt64() + y * BD.Stride;
                for ( int x = 0; x < 40; x++ )
                {
                    var L = Rand.Next( 0, 255 );
                    *( Byte* ) ( Addr + x * 3 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 1 ) = ( Byte ) L;
                    *( Byte* ) ( Addr + x * 3 + 2 ) = ( Byte ) L;
                }
            }
            inp.UnlockBits( BD );
            BD = null;
            Rand = null;
            var _Input = new Single[ inp.Width * inp.Height * 3 ];
            var Width = inp.Width;
            var Height = inp.Height;
            BD = inp.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );
            var W = inp.Width - 1;
            var inpW = Width;
            Parallel.For( 0, Height, y =>
            {
                var Address = ( Byte* ) ( BD.Scan0.ToInt64() + BD.Stride * y );
                for ( int x = 0; x < W + 1; x++ )
                {
                    var a = ( Single ) ( *Address );
                    _Input[ ( ( inpW * y ) + x ) * 3 + 2 ] = a;
                    Address += 1;
                    a = *Address;
                    _Input[ ( ( inpW * y ) + x ) * 3 + 1 ] = a;
                    Address += 1;
                    a = *Address;
                    _Input[ ( ( inpW * y ) + x ) * 3 ] = a;
                    Address += 1;
                }
            } );

            inp.UnlockBits( BD );
            BD = null;
            tmp = null;
            _Input = Methods.RGB2Y( _Input, Width, Height );

            MainProgressBar.Value += 7;

            //-> Conv1
            var temp = Methods.Conv2D9x9( _Input, ModelData.Conv1_Filters, Width, Height, 1 );
            temp = Methods.instanceNorm( temp, Width, Height, 32, ModelData.Conv1_Shift, ModelData.Conv1_Scale );
            GC.Collect();

            temp = Methods.ReLU( temp, Width, Height, 32 );
            GC.Collect();

            MainProgressBar.Value += 7;

            //-> Conv2
            temp = Methods.Conv2D3x3s2( temp, ModelData.Conv2_Filters, Width, Height, 32 );
            var _Width = Convert.ToInt32( Width / 2 );
            var _Height = Convert.ToInt32( Height / 2 );
            GC.Collect();

            temp = Methods.instanceNorm( temp, _Width, _Height, 64, ModelData.Conv2_Shift, ModelData.Conv2_Scale );
            GC.Collect();

            temp = Methods.ReLU( temp, _Width, _Height, 64 );
            GC.Collect();

            MainProgressBar.Value += 7;
            //-> Conv3
            temp = Methods.Conv2D3x3s2( temp, ModelData.Conv3_Filters, _Width, _Height, 64 );
            _Width = Convert.ToInt32( _Width / 2 );
            _Height = Convert.ToInt32( _Height / 2 );
            GC.Collect();

            temp = Methods.instanceNorm( temp, _Width, _Height, 128, ModelData.Conv3_Shift, ModelData.Conv3_Scale );
            GC.Collect();

            temp = Methods.ReLU( temp, _Width, _Height, 128 );
            GC.Collect();
            MainProgressBar.Value += 7;

            //-> ResidualBlock1
            temp = Methods.ResidualBlock( temp, _Width, _Height, ModelData.Resid1_Conv1_Shift, ModelData.Resid1_Conv1_Scale, ModelData.Resid1_Conv1_Filters, ModelData.Resid1_Conv2_Shift, ModelData.Resid1_Conv2_Scale, ModelData.Resid1_Conv2_Filters );
            MainProgressBar.Value += 7;

            //-> ResidualBlock2
            temp = Methods.ResidualBlock( temp, _Width, _Height, ModelData.Resid2_Conv1_Shift, ModelData.Resid2_Conv1_Scale, ModelData.Resid2_Conv1_Filters, ModelData.Resid2_Conv2_Shift, ModelData.Resid2_Conv2_Scale, ModelData.Resid2_Conv2_Filters );
            MainProgressBar.Value += 7;

            //-> ResidualBlock3
            temp = Methods.ResidualBlock( temp, _Width, _Height, ModelData.Resid3_Conv1_Shift, ModelData.Resid3_Conv1_Scale, ModelData.Resid3_Conv1_Filters, ModelData.Resid3_Conv2_Shift, ModelData.Resid3_Conv2_Scale, ModelData.Resid3_Conv2_Filters );
            MainProgressBar.Value += 7;

            //-> ResidualBlock4
            temp = Methods.ResidualBlock( temp, _Width, _Height, ModelData.Resid4_Conv1_Shift, ModelData.Resid4_Conv1_Scale, ModelData.Resid4_Conv1_Filters, ModelData.Resid4_Conv2_Shift, ModelData.Resid4_Conv2_Scale, ModelData.Resid4_Conv2_Filters );
            MainProgressBar.Value += 7;

            //-> ResidualBlock5
            temp = Methods.ResidualBlock( temp, _Width, _Height, ModelData.Resid5_Conv1_Shift, ModelData.Resid5_Conv1_Scale, ModelData.Resid5_Conv1_Filters, ModelData.Resid5_Conv2_Shift, ModelData.Resid5_Conv2_Scale, ModelData.Resid5_Conv2_Filters );
            MainProgressBar.Value += 7;

            //-> TransposedConv1
            temp = Methods.TransposedConv2D3x3( temp, 2, ModelData.TConv1_Filters, 64, _Width, _Height, 128 );
            _Width *= 2;
            _Height *= 2;
            GC.Collect();

            temp = Methods.instanceNorm( temp, _Width, _Height, 64, ModelData.TConv1_Shift, ModelData.TConv1_Scale );
            GC.Collect();

            temp = Methods.ReLU( temp, _Width, _Height, 64 );
            GC.Collect();

            MainProgressBar.Value += 7;

            //-> TransposedConv2
            temp = Methods.TransposedConv2D3x3( temp, 2, ModelData.TConv2_Filters, 32, _Width, _Height, 64 );
            _Width *= 2;
            _Height *= 2;
            GC.Collect();

            temp = Methods.instanceNorm( temp, _Width, _Height, 32, ModelData.TConv2_Shift, ModelData.TConv2_Scale );
            GC.Collect();

            temp = Methods.ReLU( temp, _Width, _Height, 32 );
            GC.Collect();

            MainProgressBar.Value += 7;
            //-> (Transposed)Conv(3)4
            temp = Methods.Conv2D9x9( temp, ModelData.TConv3_Filters, _Width, _Height, 32 );
            GC.Collect();

            MainProgressBar.Value += 7;
            //-> Tanh
            temp = Methods.Tanh( temp, _Width, _Height, 2 );
            GC.Collect();

            MainProgressBar.Value += 7;

            //-> BitmapNormalization
            temp = Methods.BuildRGB( temp, _Input, _Width, _Height );
            GC.Collect();

            //-> Converting to Bitmap
            var result = new Bitmap( _Width, _Height );
            BD = result.LockBits( new Rectangle( 0, 0, result.Width, result.Height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
            var Scan0 = BD.Scan0.ToInt64();
            var Stride = BD.Stride;
            W = result.Width - 1;

            Parallel.For( 0, result.Height, y =>
            {
                var Address = ( Byte* ) ( Scan0 + y * Stride );

                for ( int x = 0; x < W + 1; x++ )
                {
                    *Address = Convert.ToByte( temp[ ( ( _Width * y ) + x ) * 3 + 2 ] );
                    Address += 1;
                    *Address = Convert.ToByte( temp[ ( ( _Width * y ) + x ) * 3 + 1 ] );
                    Address += 1;
                    *Address = Convert.ToByte( temp[ ( ( _Width * y ) + x ) * 3 ] );
                    Address += 1;
                }
            } );

            result.UnlockBits( BD );
            BD = null;
            GC.Collect();

            MainProgressBar.Value += 7;
            tmp = new Bitmap( 640, 480 );
            Graphics.FromImage( tmp ).DrawImage( result, new Rectangle( 0, 0, 640, 480 ), new Rectangle( 40, 40, 640, 480 ), GraphicsUnit.Pixel );

            tmp = new Bitmap( tmp, _inputImage.Width, _inputImage.Height );

            _outputImage = new Bitmap( (Image) _inputImage );

            BD = _outputImage.LockBits( new Rectangle( 0, 0, _inputImage.Width, _inputImage.Height ), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

            var BD1 = tmp.LockBits( new Rectangle( 0, 0, tmp.Width, tmp.Height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            Scan0 = BD.Scan0.ToInt64();
            Stride = BD.Stride;
            W = tmp.Width - 1;

            var Scan0_1 = BD1.Scan0.ToInt64();
            var Stride_1 = BD1.Stride;

            Parallel.For( 0, tmp.Height, y =>
            {
                var AddressStylized = ( Byte* ) ( Scan0 + y * Stride );
                var AddressOrigin = ( Byte* ) ( Scan0_1 + y * Stride_1 );

                for ( int x = 0; x < W + 1; x++ )
                {
                    var Y_B = *AddressStylized;
                    AddressStylized += 1;
                    var Y_G = *AddressStylized;
                    AddressStylized += 1;
                    var Y_R = *AddressStylized;
                    AddressStylized -= 2;
                    var ContentY = 0.299f * Y_R + 0.587f * Y_G + 0.114f * Y_B;
                    var O_B = *AddressOrigin;
                    AddressOrigin += 1;
                    var O_G = *AddressOrigin;
                    AddressOrigin += 1;
                    var O_R = *AddressOrigin;
                    AddressOrigin += 1;
                    var OriginU = -0.14713f * O_R - 0.28886f * O_G + 0.436f * O_B + 128.0f;
                    var OriginV = 0.615f * O_R - 0.51499f * O_G - 0.10001f * O_B + 128.0f;
                    var R = Convert.ToInt16( ContentY + 1.13983f * ( OriginV - 128.0f ) );

                    if ( R > 255 )
                    {
                        R = 255;
                    }
                    else
                    {
                        if ( R < 0 )
                        {
                            R = 0;
                        }
                    }

                    var G = Convert.ToInt16( ContentY - 0.39465f * ( OriginU - 128.0f ) - 0.58060f * ( OriginV - 128.0f ) );

                    if ( G > 255 )
                    {
                        G = 255;
                    }
                    else
                    {
                        if ( G < 0 )
                        {
                            G = 0;
                        }
                    }

                    var B = Convert.ToInt16( ContentY + 2.03211f * ( OriginU - 128.0f ) );

                    if ( B > 255 )
                    {
                        B = 255;
                    }
                    else
                    {
                        if ( B < 0 )
                        {
                            B = 0;
                        }
                    }
                    *AddressStylized = ( Byte ) B;
                    AddressStylized += 1;
                    *AddressStylized = ( Byte ) G;
                    AddressStylized += 1;
                    *AddressStylized = ( Byte ) R;
                    AddressStylized += 1;
                };
            } );

            _outputImage.UnlockBits( BD );
            tmp.UnlockBits( BD1 );

            BD = null;
            BD1 = null;

            _outputImage = Methods.RestoreColors( _outputImage );
            MainProgressBar.Value = 0;
            MainPictureBox.Image = _outputImage;

            SaveImageToolStripMenuItem.Enabled = true;
            OpenImageToolStripMenuItem.Enabled = true;

            StartStopToolStripMenuItem.Text = "Старт";

            StartStopToolStripMenuItem.Click -= StopColorization;
            StartStopToolStripMenuItem.Click += StartColorization;

            MainProgressBar.Visible = false;

            Text = "PABCSoft - FastNeuralColor: Завершено за " + ( DateTime.Now - timer ).TotalSeconds + " секунд!";
        }

        private void ColorizeImage()
        {
            Invoke( new Action( Colorize ) );
        }

        private void StartColorization( object sender, EventArgs e )
        {
            ColorizationThread = new Thread( ColorizeImage );
            ColorizationThread.Start();
        }
    }
}
