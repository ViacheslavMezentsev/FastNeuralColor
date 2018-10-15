//***************************************************************************
// !! При компиляции ОТКЛЮЧИТЬ генерацию отладочной информации(Debug)!
// !! Работа программы в Release будет ощутимо лучше.
//***************************************************************************
//---------------------------------------------------------------------------
//-> Using's
//---------------------------------------------------------------------------
using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.IO.Compression;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//---------------------------------------------------------------------------
//-> Copyrights
//---------------------------------------------------------------------------
[assembly: AssemblyTitle("FastNeuralColor")]
[assembly: AssemblyDescription("Image Colorization")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyVersion("24.07.2018")]
[assembly: AssemblyCompany("PABCSoft")]
[assembly: AssemblyProduct("FastNeuralColor")]
[assembly: AssemblyCopyright("© Брыкин Глеб Сергеевич")]
[assembly: AssemblyTrademark("ImageInstruments")]
[assembly: AssemblyCulture("")]
namespace FastNeuralColor{
  //---------------------------------------------------------------------------
  //-> System implementations
  //---------------------------------------------------------------------------
  sealed class ImageComparerBox:System.Windows.Forms.PictureBox{
    private System.Drawing.Bitmap BMP;
    private System.Drawing.Bitmap _Image1;
    private System.Drawing.Bitmap _Image2;
    private System.Drawing.Graphics G;
    private System.Int32 x;
    private System.Drawing.Bitmap __Image1;
    private System.Drawing.Bitmap __Image2;
    public ImageComparerBox(System.Int32 w, System.Int32 h){
      this.BackColor = System.Drawing.Color.White;
      this.Width = w;
      this.Height = h;
      this.Image = this.BMP;
      this.x = w / 2;
      this.MouseDown += this.Fix;
      this.MouseMove += this.Draw;
      this.Cursor = System.Windows.Forms.Cursors.VSplit;
      this.Resize += this.Resizeing;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
    }
    private void Resizeing(System.Object sender, System.EventArgs E){
      this.x = this.Width / 2;
      this.SetImages(this.__Image1, this.__Image2, this.__Image1.Size);
      this.Redraw();
    }
    private void Draw(System.Object sender, System.Windows.Forms.MouseEventArgs E){
      if (E.Button == System.Windows.Forms.MouseButtons.Left){
        x = System.Convert.ToInt32(E.X - ((this.Width - this.BMP.Width) / 2));
        this.Redraw();
      }
    }
    private void Fix(System.Object sender, System.Windows.Forms.MouseEventArgs E){
      if (E.Button == System.Windows.Forms.MouseButtons.Left){
        x = System.Convert.ToInt32(E.X - ((this.Width - this.BMP.Width) / 2));
        this.Redraw();
      }
    }
    private void Redraw(){
      this.G.DrawImage(this._Image1, new System.Drawing.Rectangle(0, 0, x, this.Height), new System.Drawing.Rectangle(0, 0, x, this.Height), System.Drawing.GraphicsUnit.Pixel);
      this.G.DrawImage(this._Image2, new System.Drawing.Rectangle(x, 0, this.Width - x, this.Height), new System.Drawing.Rectangle(x, 0, this.Width - x, this.Height), System.Drawing.GraphicsUnit.Pixel);
      this.G.FillRectangle(System.Drawing.Brushes.Magenta, x - 1, 0, 2, this.Height);
      this.Invalidate();
    }
    public void SetImages(System.Drawing.Bitmap Image1, System.Drawing.Bitmap Image2, System.Drawing.Size DestSize){
      this.__Image1 = new System.Drawing.Bitmap(Image1, DestSize);
      this.__Image2 = new System.Drawing.Bitmap(Image2, DestSize);
      if ((DestSize.Width <= this.Width) && (DestSize.Height <= this.Height)){
        this._Image1 = new System.Drawing.Bitmap(this.__Image1);
        this._Image2 = new System.Drawing.Bitmap(this.__Image2);
      }
      else{
        if ((DestSize.Width <= this.Width) && (DestSize.Height > this.Height)){
          var k = this.Height / DestSize.Height;
          this._Image1 = new System.Drawing.Bitmap(this.__Image1, System.Convert.ToInt32(DestSize.Width * k), System.Convert.ToInt32(DestSize.Height * k));
          this._Image2 = new System.Drawing.Bitmap(this.__Image2, System.Convert.ToInt32(DestSize.Width * k), System.Convert.ToInt32(DestSize.Height * k));
        }
        else{
          if ((DestSize.Width > this.Width) && (DestSize.Height <= this.Height)){
          var k = (System.Double)this.Width / DestSize.Width;
          this._Image1 = new System.Drawing.Bitmap(this.__Image1, System.Convert.ToInt32(DestSize.Width * k), System.Convert.ToInt32(DestSize.Height * k));
          this._Image2 = new System.Drawing.Bitmap(this.__Image2, System.Convert.ToInt32(DestSize.Width * k), System.Convert.ToInt32(DestSize.Height * k));
          }
          else{
            var k1 = (System.Double)this.Width / DestSize.Width;
            var k2 = (System.Double)this.Height / DestSize.Height;
            if (k1 > k2){
              this._Image1 = new System.Drawing.Bitmap(this.__Image1, System.Convert.ToInt32(DestSize.Width * k2), System.Convert.ToInt32(DestSize.Height * k2));
              this._Image2 = new System.Drawing.Bitmap(this.__Image2, System.Convert.ToInt32(DestSize.Width * k2), System.Convert.ToInt32(DestSize.Height * k2));
            }
            else{
              this._Image1 = new System.Drawing.Bitmap(this.__Image1, System.Convert.ToInt32(DestSize.Width * k1), System.Convert.ToInt32(DestSize.Height * k1));
              this._Image2 = new System.Drawing.Bitmap(this.__Image2, System.Convert.ToInt32(DestSize.Width * k1), System.Convert.ToInt32(DestSize.Height * k1));
            }
          }
        }
      }
      this.BMP = new System.Drawing.Bitmap(this._Image1);
      this.BackgroundImage = this.BMP;
      this.G = System.Drawing.Graphics.FromImage(this.BMP);
      this.Redraw();
      System.GC.Collect();
    }
  }
  
  sealed class PreviewForm:System.Windows.Forms.Form{
    private ImageComparerBox CompareBox;
    private System.Windows.Forms.Button SaveButton;
    public PreviewForm(){
      this.Icon = System.Drawing.Icon.FromHandle((new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Weights.jpg"))).GetHicon());
      this.ClientSize = new System.Drawing.Size(640, 480);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Text = "Предпросмотр";
      this.CompareBox = new ImageComparerBox(640, 440);
      this.CompareBox.SetImages(Application.InputImage, Application.OutputImage, Application.InputImage.Size);
      this.Controls.Add(this.CompareBox);
      this.SaveButton = new System.Windows.Forms.Button();
      this.SaveButton.Top = 440;
      this.SaveButton.Size = new System.Drawing.Size(640, 40);
      this.SaveButton.Text = "Сохранить";
      this.SaveButton.Click += this.SaveImage;
      this.Controls.Add(this.SaveButton);
      this.Closing += this.Dispose;
    }
    private void Dispose(System.Object Sender, System.ComponentModel.CancelEventArgs E){
      this.SaveButton = null;
      this.CompareBox = null;
      System.GC.Collect();
      this.Dispose();
    }
    private void SaveImage(System.Object sender, System.EventArgs E){
      System.Windows.Forms.SaveFileDialog SaveImageDialog;
      SaveImageDialog = new System.Windows.Forms.SaveFileDialog();
      SaveImageDialog.Title = "Save image";
      SaveImageDialog.Filter = "Images (*.bmp)|*.bmp|Images (*.emf)|*.emf|Images (*.exif)|*.exif|Images (*.gif)|*.gif|Images (*.ico)|*.ico|Images (*.jpg)|*.jpg|Images (*.png)|*.png|Images (*.tiff)|*.tiff|Images (*.wmf)|*.wmf";
      if (SaveImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK){
        try{
          switch(SaveImageDialog.FilterIndex){
            case 1:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
              break;
            }
            case 2:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Emf);
              break;
            }
            case 3:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Exif);
              break;
            }
            case 4:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Gif);
              break;
            }
            case 5:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Icon);
              break;
            }
            case 6:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
              break;
            }
            case 7:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
              break;
            }
            case 8:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
              break;
            }
            case 9:{
              Application.OutputImage.Save(SaveImageDialog.FileName, System.Drawing.Imaging.ImageFormat.Wmf);
              break;
            }
          }
        }
        catch(System.Exception ex){
          System.Windows.Forms.MessageBox.Show("Failed to save image.", "Error!");
        }
      }
    }
  }
  public unsafe static class Methods{
    public static System.Drawing.Bitmap RestoreColors(System.Drawing.Bitmap BMP){
      var Result = new System.Drawing.Bitmap(BMP.Width, BMP.Height);
      var arrRed = new System.UInt16[256];
      var arrGreen = new System.UInt16[256];
      var arrBlue = new System.UInt16[256];
      var Width = BMP.Width;
      var Height = BMP.Height;
      var BD = BMP.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      System.Threading.Tasks.Parallel.For(0, Height, (System.Int32 y)=>{
        var Address = (System.Byte*)(BD.Scan0.ToInt32() + y * BD.Stride);
        for(System.Int32 x = 0; x < Width; x++){
          lock (arrBlue){
            arrBlue[*Address] += 1;
          }
          Address += 1;
          lock (arrGreen){
            arrGreen[*Address] += 1;
          }
          Address += 1;
          lock (arrRed){
            arrRed[*Address] += 1;
          }
          Address += 1;
        }
      });
      System.Double q = Width * Height * 0.01;
      System.UInt16 newMinR = 0;
      System.UInt16 newMaxR = 0;
      System.UInt16 newMinG = 0;
      System.UInt16 newMaxG = 0;
      System.UInt16 newMinB = 0;
      System.UInt16 newMaxB = 0;
      System.UInt64 s = 0;
      for(System.Byte i = 0; i <= 255; i++){
        s += arrRed[i];
        if (s >= q){
          newMinR = i;
          break;
        }
      }
      s = 0;
      for(System.Byte i = 255; i >= 0; i--){
        s += arrRed[i];
        if (s >= q){
          newMaxR = i;
          break;
        }
      }
      s = 0;
      for(System.Byte i = 0; i <= 255; i++){
        s += arrGreen[i];
        if (s >= q){
          newMinG = i;
          break;
        }
      }
      s = 0;
      for(System.Byte i = 255; i >= 0; i--){
        s += arrGreen[i];
        if (s >= q){
          newMaxG = i;
          break;
        }
      }
      s = 0;
      for(System.Byte i = 0; i <= 255; i++){
        s += arrBlue[i];
        if (s >= q){
          newMinB = i;
          break;
        }
      }
      s = 0;
      for(System.Byte i = 255; i >= 0; i--){
        s += arrBlue[i];
        if (s >= q){
          newMaxB = i;
          break;
        }
      }
      var BW_BD = Result.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      System.Threading.Tasks.Parallel.For(0, Height, (System.Int32 y)=>{
        System.Byte* BW_Address;
        System.Byte* In_Address;
        System.Int16 newR = 0;
        System.Int16 newG = 0;
        System.Int16 newB = 0;
        In_Address = (System.Byte*)(BD.Scan0.ToInt32() + y * BD.Stride);
        BW_Address = (System.Byte*)(BW_BD.Scan0.ToInt32() + y * BW_BD.Stride);
        for(System.Int32 x = 0; x < Width; x++){
          newB = System.Convert.ToInt16(System.Math.Round((System.Single)(*In_Address - newMinB) * 255.0f / (newMaxB - newMinB+1)));
          if (newB < 0){
            newB = 0;
          }
          if (newB > 255){
            newB = 255;
          }
          In_Address+=1;
          newG = System.Convert.ToInt16(System.Math.Round((System.Single)(*In_Address - newMinG) * 255.0f / (newMaxG - newMinG+1)));
          if(newG < 0){
            newG = 0;
          }
          if(newG > 255){
            newG = 255;
          }
          In_Address+=1;
          newR = System.Convert.ToInt16(System.Math.Round((System.Single)(*In_Address - newMinR) * 255.0f / (newMaxR - newMinR+1)));
          if (newR < 0){
            newR = 0;
          }
          if (newR > 255){
            newR = 255;
          }
          In_Address+=1;
          *BW_Address = (System.Byte)newB;
          BW_Address += 1;
          *BW_Address = (System.Byte)newG;
          BW_Address += 1;
          *BW_Address = (System.Byte)newR;
          BW_Address += 1;
        }
      });
      BMP.UnlockBits(BD);
      Result.UnlockBits(BW_BD);
      arrRed = null;
      arrGreen = null;
      arrBlue = null;
      System.GC.Collect();
      return Result;
    }
    public static System.Single[] RGB2Y(System.Single[] RGB, System.Int32 W, System.Int32 H){
      var Result = new System.Single[W * H];
      System.Threading.Tasks.Parallel.For(0, H, (System.Int32 y)=>{
        for(System.Int32 x = 0; x < W; x++){
          Result[(W * y) + x] = (RGB[((W * y) + x) * 3] * 0.299f + RGB[((W * y) + x) * 3 + 1] * 0.587f + RGB[((W * y) + x) * 3 + 2] * 0.114f) / 255.0f - 0.5f;
        }
      });
      return Result;
    }
    public static System.Single[] ReLU(System.Single[] input, System.Int32 w, System.Int32 h, System.Int32 d){
      var Temp = new System.Single[w * h * d];
      System.Threading.Tasks.Parallel.For(0, d, (System.Int32 _d)=>{
      for(System.Int32 _y = 0; _y < h; _y++){
        for(System.Int32 _x = 0; _x < w; _x++){
            var v = input[((w * _y) + _x) * d + _d];
            Temp[((w * _y) + _x) * d + _d] = (v < 0) ? (0.0f) : (v);
          }
        }
      });
      return Temp;
    }
    public static System.Single[] Conv2D3x3s1(System.Single[] input, System.Single[][] Filters, System.Int32 InputWidth, System.Int32 InputHeight, System.Int32 InputDepth){
      var OutputWidth = System.Convert.ToInt32((InputWidth + 2 - 3) + 1);
      var OutputHeight = System.Convert.ToInt32((InputHeight + 2 - 3) + 1);
      var OutputDepth = Filters.Length;
      var tempOutput = new System.Single[OutputWidth * OutputHeight * OutputDepth];
      var xy_stride = 1;
      System.Threading.Tasks.Parallel.For(0, OutputDepth, (System.Int32 d)=>{
        var f = Filters[d];
        for(System.Int32 ay = 0; ay < OutputHeight; ay++){
          var y = (-1 | 0) + (xy_stride * ay);
          for(System.Int32 ax = 0; ax < OutputWidth; ax++){
            var x = (-1 | 0) + (xy_stride * ax);
            var a = 0.0f;
            for(System.Byte fy = 0; fy < 3; fy++){
              var oy = y + fy;
              for(System.Byte fx = 0; fx < 3; fx++){
                var ox = x + fx;
                if ((oy >= 0) && (oy < InputHeight) && (ox >= 0) && (ox < InputWidth)){
                  for(System.Byte fd = 0; fd < InputDepth; fd++){
                    a += f[((3 * fy) + fx) * InputDepth + fd] * input[((InputWidth * oy) + ox) * InputDepth + fd];
                  }
                }
              }
            }
            tempOutput[((OutputWidth * ay) + ax) * OutputDepth + d] = a;
          }
        }
      });
      return tempOutput;
    }
    public static System.Single[] Conv2D3x3s2(System.Single[] input, System.Single[][] Filters, System.Int32 InputWidth, System.Int32 InputHeight, System.Int32 InputDepth){
      var OutputWidth = System.Convert.ToInt32((InputWidth + 2 - 3) / 2 + 1);
      var OutputHeight = System.Convert.ToInt32((InputHeight + 2 - 3) / 2 + 1);
      var OutputDepth = Filters.Length;
      var tempOutput = new System.Single[OutputWidth * OutputHeight * OutputDepth];
      var xy_stride = 2;
      System.Threading.Tasks.Parallel.For(0, OutputDepth, (System.Int32 d)=>{
        var f = Filters[d];
        for(System.Int32 ay = 0; ay < OutputHeight; ay++){
          var y = (-1 | 0) + (xy_stride * ay);
          for(System.Int32 ax = 0; ax < OutputWidth; ax++){
            var x = (-1 | 0) + (xy_stride * ax);
            var a = 0.0f;
            for(System.Byte fy = 0; fy < 3; fy++){
              var oy = y + fy;
              for(System.Byte fx = 0; fx < 3; fx++){
                var ox = x + fx;
                if ((oy >= 0) && (oy < InputHeight) && (ox >= 0) && (ox < InputWidth)){
                  for(System.Byte fd = 0; fd < InputDepth; fd++){
                    a += f[((3 * fy) + fx) * InputDepth + fd] * input[((InputWidth * oy) + ox) * InputDepth + fd];
                  }
                }
              }
            }
            tempOutput[((OutputWidth * ay) + ax) * OutputDepth + d] = a;
          }
        }
      });
      return tempOutput;
    }
   public static System.Single[] Conv2D9x9(System.Single[] input, System.Single[][] Filters, System.Int32 InputWidth, System.Int32 InputHeight, System.Int32 InputDepth){
     var OutputWidth = System.Convert.ToInt32((InputWidth + 8 - 9) + 1);
     var OutputHeight = System.Convert.ToInt32((InputHeight + 8 - 9) + 1);
     var OutputDepth = Filters.Length;
     var tempOutput = new System.Single[OutputWidth * OutputHeight * OutputDepth];
     var xy_stride = 1;
     System.Threading.Tasks.Parallel.For(0, OutputDepth, (System.Int32 d)=>{
       var f = Filters[d];
       for(System.Int32 ay = 0; ay < OutputHeight; ay++){
         var y = (-4 | 0) + (xy_stride * ay);
         for(System.Int32 ax = 0; ax < OutputWidth; ax++){
           var x = (-4 | 0) + (xy_stride * ax);
           var a = 0.0f;
           for(System.Byte fy = 0; fy < 9; fy++){
             var oy = y + fy;
             for(System.Byte fx = 0; fx < 9; fx++){
               var ox = x + fx;
               if ((oy >= 0) && (oy < InputHeight) && (ox >= 0) && (ox < InputWidth)){
                 for(System.Byte fd = 0; fd < InputDepth; fd++){
                   a += f[((9 * fy) + fx) * InputDepth + fd] * input[((InputWidth * oy) + ox) * InputDepth + fd];
                 }
               }
             }
           }
           tempOutput[((OutputWidth * ay) + ax) * OutputDepth + d] = a;
         }
       }
     });
     return tempOutput;
   }
    public static System.Single[] TransposedConv2D3x3(System.Single[] input, System.Int32 Stride, System.Single[][] Filters, System.Int32 FilterDepth, System.Int32 InputWidth, System.Int32 InputHeight, System.Int32 InputDepth){
      var OutputWidth = InputWidth * Stride;
      var OutputHeight = InputHeight * Stride;
      var OutputDepth = FilterDepth;
      var tempOutput = new System.Single[OutputWidth * OutputHeight * OutputDepth];
      for(System.Int32 y = 0; y < InputHeight; y++){
        for(System.Int32 x = 0; x < InputWidth; x++){
          for(System.Int32 d = 0; d < Filters.Length; d++){
            var Filter = Filters[d];
            for(System.Byte fd = 0; fd < FilterDepth; fd++){
              for(System.Byte fy = 0; fy < 3; fy++){
                for(System.Byte fx = 0; fx < 3; fx++){
                  var ox = x * Stride + fx;
                  var oy = y * Stride + fy;
                  if((ox < OutputWidth) && (oy < OutputHeight)){
                    tempOutput[((OutputWidth * oy) + ox) * OutputDepth + fd] = input[((InputWidth * y) + x) * InputDepth + d] * Filter[((3 * fy) + fx) * FilterDepth + fd] + tempOutput[((OutputWidth * oy) + ox) * OutputDepth + fd];
                  }
                }
              }
            }
          }
        }
      }
      return tempOutput;
    }
    public static System.Single[] instanceNorm(System.Single[] input, System.Int32 InputWidth, System.Int32 InputHeight, System.Int32 InputDepth, System.Single[] Shift, System.Single[] Scale){
      var _Mean = new System.Single[InputDepth];
      var Variance = new System.Single[InputDepth];
      for(System.Int32 d = 0; d < InputDepth; d++){
        System.Double Temp = 0.0;
        for(System.Int32 y = 0; y < InputHeight; y++){
          for(System.Int32 x = 0; x < InputWidth; x++){
            Temp += input[((InputWidth * y) + x) * InputDepth + d];
          }
        }
        Temp /= (System.Double)InputHeight * InputWidth;
        _Mean[d] = (System.Single)Temp;
        System.Double Temp2 = 0.0;
        for(System.Int32 y = 0; y < InputHeight; y++){
          for(System.Int32 x = 0; x < InputWidth; x++){
            var _x = input[((InputWidth * y) + x) * InputDepth + d] - Temp;
            Temp2 += _x * _x;
          }
        }
        Temp2 /= (System.Double)InputHeight * InputWidth;
        Variance[d] = (System.Single)Temp2;
      }
      var _Temp = new System.Single[InputWidth * InputHeight * InputDepth];
      var inp = input;
      System.Threading.Tasks.Parallel.For(0, InputDepth, (System.Int32 d)=>{
        for(System.Int32 y = 0; y < InputHeight; y++){
          for(System.Int32 x = 0; x < InputWidth; x++){
            _Temp[((InputWidth * y) + x) * InputDepth + d] = (System.Single)(((inp[((InputWidth * y) + x) * InputDepth + d] - _Mean[d]) / System.Math.Sqrt(0.001f + Variance[d])) * Scale[d] + Shift[d]);
          }
        }
      });
      return _Temp;
    }
    public static System.Single[] ResidualBlock(System.Single[] input, System.Int32 InputWidth, System.Int32 InputHeight, System.Single[] Shift1, System.Single[] Scale1, System.Single[][] Filters1, System.Single[] Shift2, System.Single[] Scale2, System.Single[][] Filters2){
      var Conv1 = Conv2D3x3s1(input, Filters1, InputWidth, InputHeight, 128);
      Conv1 = instanceNorm(Conv1, InputWidth, InputHeight, 128, Shift1, Scale1);
      Conv1 = ReLU(Conv1, InputWidth, InputHeight, 128);
      var Conv2 = Conv2D3x3s1(Conv1, Filters2, InputWidth, InputHeight, 128);
      Conv2 = instanceNorm(Conv2, InputWidth, InputHeight, 128, Shift2, Scale2);
      var Temp = new System.Single[InputWidth * InputHeight * 128];
      var inp = input;
      System.Threading.Tasks.Parallel.For(0, 128, (System.Int32 d)=>{
        for(System.Int32 y = 0; y < InputHeight; y++){
          for(System.Int32 x = 0; x < InputWidth; x++){
            Temp[((InputWidth * y) + x) * 128 + d] = inp[((InputWidth * y) + x) * 128 + d] + Conv2[((InputWidth * y) + x) * 128 + d];
          }
        }
      });
      Conv1 = null;
      Conv2 = null;
      System.GC.Collect();
      return Temp;
    }
    public static System.Single[] Tanh(System.Single[] input, System.Int32 InputWidth, System.Int32 InputHeight, System.Int32 InputDepth){
      var Temp = new System.Single[InputWidth * InputHeight * InputDepth];
      var inp = input;
      System.Threading.Tasks.Parallel.For(0, InputDepth, (System.Int32 d)=>{
        for(System.Int32 y = 0; y < InputHeight; y++){
          for(System.Int32 x = 0; x < InputWidth; x++){
            Temp[((InputWidth * y) + x) * InputDepth + d] = (System.Single)System.Math.Tanh(inp[((InputWidth * y) + x) * InputDepth + d]);
          }
        }
      });
      return Temp;
    }
    public static System.Single[] BuildRGB(System.Single[] inputUV, System.Single[] inputY, System.Int32 InputWidth, System.Int32 InputHeight){
      var Result = new System.Single[InputWidth * InputHeight * 3];
      var min = 99999999.0f;
      var max = 0.0f;
      for(System.UInt16 x = 0; x < InputWidth; x++){
        for(System.UInt16 y = 0; y < InputHeight; y++){
          var v = inputUV[((InputWidth * y) + x) * 2 + 0];
          if (v < min){
            min = v;
          }
          if (v > max){
            max = v;
          }
          v = inputUV[((InputWidth * y) + x) * 2 + 1];
          if (v < min){
            min = v;
          }
          if (v > max){
            max = v;
          }
        }
      }
      for(System.Int32 x = 0; x < InputWidth; x++){
        for(System.Int32 y = 0; y < InputHeight; y++){
          System.Single _Y = (inputY[((InputWidth * y) + x)] + 0.5f) * 255.0f;
          System.Single _U = ((inputUV[((InputWidth * y) + x) * 2 + 0] * 0.436f - min) / (max - min)) * 239.17f;
          System.Single _V = ((inputUV[((InputWidth * y) + x) * 2 + 1] * 0.615f - min) / (max - min)) * 313.65f - 28.8f;
          System.Single r = _Y + (1.4075f * (_V - 128.0f));
          System.Single g = _Y - (0.3455f * (_U - 128.0f) - (0.7169f * (_V - 128.0f)));
          System.Single b = _Y + (1.7790f * (_U - 128.0f));
          if (r < 0.0f){
            r = 0.0f;
          }
          if (r > 255.0f){
            r = 255.0f;
          }
          if (g < 0.0f){
            g = 0.0f;
          }
          if (g > 255.0f){
            g = 255.0f;
          }
          if (b < 0.0f){
            b = 0.0f;
          }
          if (b > 255.0f){
            b = 255.0f;
          }
          Result[((InputWidth * y) + x) * 3 + 0] = r;
          Result[((InputWidth * y) + x) * 3 + 1] = g;
          Result[((InputWidth * y) + x) * 3 + 2] = b;
        }
      }
      return Result;
    }
  }
  public sealed class Data{
    public System.Single[][] Conv1_Filters;
    public System.Single[] Conv1_Shift;
    public System.Single[] Conv1_Scale;
    public System.Single[][] Conv2_Filters;
    public System.Single[] Conv2_Shift;
    public System.Single[] Conv2_Scale;
    public System.Single[][] Conv3_Filters;
    public System.Single[] Conv3_Shift;
    public System.Single[] Conv3_Scale;
    //-> Residual
    public System.Single[][] Resid1_Conv1_Filters;
    public System.Single[] Resid1_Conv1_Shift;
    public System.Single[] Resid1_Conv1_Scale;
    public System.Single[][] Resid1_Conv2_Filters;
    public System.Single[] Resid1_Conv2_Shift;
    public System.Single[] Resid1_Conv2_Scale;
    public System.Single[][] Resid2_Conv1_Filters;
    public System.Single[] Resid2_Conv1_Shift;
    public System.Single[] Resid2_Conv1_Scale;
    public System.Single[][] Resid2_Conv2_Filters;
    public System.Single[] Resid2_Conv2_Shift;
    public System.Single[] Resid2_Conv2_Scale;
    public System.Single[][] Resid3_Conv1_Filters;
    public System.Single[] Resid3_Conv1_Shift;
    public System.Single[] Resid3_Conv1_Scale;
    public System.Single[][] Resid3_Conv2_Filters;
    public System.Single[] Resid3_Conv2_Shift;
    public System.Single[] Resid3_Conv2_Scale;
    public System.Single[][] Resid4_Conv1_Filters;
    public System.Single[] Resid4_Conv1_Shift;
    public System.Single[] Resid4_Conv1_Scale;
    public System.Single[][] Resid4_Conv2_Filters;
    public System.Single[] Resid4_Conv2_Shift;
    public System.Single[] Resid4_Conv2_Scale;
    public System.Single[][] Resid5_Conv1_Filters;
    public System.Single[] Resid5_Conv1_Shift;
    public System.Single[] Resid5_Conv1_Scale;
    public System.Single[][] Resid5_Conv2_Filters;
    public System.Single[] Resid5_Conv2_Shift;
    public System.Single[] Resid5_Conv2_Scale;
    //-> Deconv
    public System.Single[][] TConv1_Filters;
    public System.Single[] TConv1_Shift;
    public System.Single[] TConv1_Scale;
    public System.Single[][] TConv2_Filters;
    public System.Single[] TConv2_Shift;
    public System.Single[] TConv2_Scale;
    public System.Single[][] TConv3_Filters;
    private static System.Single[][] ReadFilters(System.Byte n, System.Byte d, System.Byte h, System.Byte w, System.IO.BinaryReader br){
      var Result = new System.Single[n][];
      for(System.Int32 _n = 0; _n < n; _n++){
        Result[_n] = new System.Single[w * h * d];
      }
      for(System.Int32 _n = 0; _n < n; _n++){
        for(System.Int32 _d = 0; _d < d; _d++){
          for(System.Int32 _h = 0; _h < h; _h++){
            for(System.Int32 _w = 0; _w < w; _w++){
              Result[_n][((w * _h) + _w) * d + _d] = br.ReadSingle();
            }
          }
        }
      }
      return Result;
    }
    private static System.Single[] ReadLinear(System.Byte n, System.IO.BinaryReader br){
      var Result = new System.Single[n];
      for(System.Int32 i = 0; i < n; i++){
        Result[i] = br.ReadSingle();
      }
      return Result;
    }
    public Data(System.IO.Stream BaseStream){
      var fs = new System.IO.BinaryReader(BaseStream);
      Conv1_Filters = ReadFilters(32, 1, 9, 9, fs);
      Conv1_Shift = ReadLinear(32, fs);
      Conv1_Scale = ReadLinear(32, fs);
      Conv2_Filters = ReadFilters(64, 32, 3, 3, fs);
      Conv2_Shift = ReadLinear(64, fs);
      Conv2_Scale = ReadLinear(64, fs);
      Conv3_Filters = ReadFilters(128, 64, 3, 3, fs);
      Conv3_Shift = ReadLinear(128, fs);
      Conv3_Scale = ReadLinear(128, fs);
      Resid1_Conv1_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid1_Conv1_Shift = ReadLinear(128, fs);
      Resid1_Conv1_Scale = ReadLinear(128, fs);
      Resid1_Conv2_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid1_Conv2_Shift = ReadLinear(128, fs);
      Resid1_Conv2_Scale = ReadLinear(128, fs);
      Resid2_Conv1_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid2_Conv1_Shift = ReadLinear(128, fs);
      Resid2_Conv1_Scale = ReadLinear(128, fs);
      Resid2_Conv2_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid2_Conv2_Shift = ReadLinear(128, fs);
      Resid2_Conv2_Scale = ReadLinear(128, fs);
      Resid3_Conv1_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid3_Conv1_Shift = ReadLinear(128, fs);
      Resid3_Conv1_Scale = ReadLinear(128, fs);
      Resid3_Conv2_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid3_Conv2_Shift = ReadLinear(128, fs);
      Resid3_Conv2_Scale = ReadLinear(128, fs);
      Resid4_Conv1_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid4_Conv1_Shift = ReadLinear(128, fs);
      Resid4_Conv1_Scale = ReadLinear(128, fs);
      Resid4_Conv2_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid4_Conv2_Shift = ReadLinear(128, fs);
      Resid4_Conv2_Scale = ReadLinear(128, fs);
      Resid5_Conv1_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid5_Conv1_Shift = ReadLinear(128, fs);
      Resid5_Conv1_Scale = ReadLinear(128, fs);
      Resid5_Conv2_Filters = ReadFilters(128, 128, 3, 3, fs);
      Resid5_Conv2_Shift = ReadLinear(128, fs);
      Resid5_Conv2_Scale = ReadLinear(128, fs);
      TConv1_Filters = ReadFilters(128, 64, 3, 3, fs);
      TConv1_Shift = ReadLinear(64, fs);
      TConv1_Scale = ReadLinear(64, fs);
      TConv2_Filters = ReadFilters(64, 32, 3, 3, fs);
      TConv2_Shift = ReadLinear(32, fs);
      TConv2_Scale = ReadLinear(32, fs);
      TConv3_Filters = ReadFilters(2, 32, 9, 9, fs);
      fs.Close();
    }
  }
  sealed class MainForm:System.Windows.Forms.Form{
    private Data ModelData;
    private System.Windows.Forms.ToolStrip MainToolStrip;
    private System.Windows.Forms.ToolStripMenuItem ImageToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem OpenImageToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem SaveImageToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem StartStopToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
    private System.Windows.Forms.PictureBox MainPictureBox;
    private System.Windows.Forms.ProgressBar MainProgressBar;
    private System.Threading.Thread ColorizationThread;
    private static System.IO.MemoryStream Decompress(System.IO.Stream Input){
      var Result = new System.IO.MemoryStream((System.Int32)Application.OriginalModelSize);
      var Decompressor = new System.IO.Compression.GZipStream(Input, System.IO.Compression.CompressionMode.Decompress);
      var Buffer = new System.Byte[Application.OriginalModelSize];
      Decompressor.Read(Buffer, 0, Buffer.Length);
      Result.Write(Buffer, 0, Buffer.Length);
      Buffer = null;
      Decompressor = null;
      Result.Position = 0;
      System.GC.Collect();
      return Result;
    }
    private unsafe void OpenImage(System.Object sender, System.EventArgs E){
      var OpenImageDialog = new System.Windows.Forms.OpenFileDialog();
      OpenImageDialog.Title = "Select image to coloring";
      OpenImageDialog.Filter = "Images (*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf)|*.bmp; *.emf; *.exif; *.gif; *.ico; *.jpg; *.png; *.tiff; *.wmf|All types(*.*)|*.*";
      if (OpenImageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK){
        Application.InputImage = new System.Drawing.Bitmap(OpenImageDialog.FileName);
        var BD = Application.InputImage.LockBits(new System.Drawing.Rectangle(0, 0, Application.InputImage.Width, Application.InputImage.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        for(System.Int32 y = 0; y < Application.InputImage.Height; y++){
          var Addr = BD.Scan0.ToInt32() + BD.Stride * y;
          for(System.Int32 x = 0; x < Application.InputImage.Width; x++){
            var AddrR = (System.Byte*)(Addr + 2);
            var AddrG = (System.Byte*)(Addr + 1);
            var AddrB = (System.Byte*)(Addr);
            var L = System.Convert.ToByte((*AddrR + *AddrG + *AddrB) / 3);
            *AddrR = L;
            *AddrG = L;
            *AddrB = L;
            Addr += 3;
          }
        }
        Application.InputImage.UnlockBits(BD);
        BD = null;
        this.MainPictureBox.Image = Application.InputImage;
        this.StartStopToolStripMenuItem.Enabled = true;
        this.SaveImageToolStripMenuItem.Enabled = false;
      }
    }
    private void SaveImage(System.Object sender, System.EventArgs E){
      (new PreviewForm()).ShowDialog();
    }
    private void Close(System.Object sender, System.EventArgs E){
      if(this.ColorizationThread != null){
        if(this.ColorizationThread.IsAlive == true){
          this.ColorizationThread.Abort();
        }
      }
      this.Close();
    }
    private void CloseSender(System.Object sender, System.ComponentModel.CancelEventArgs E){
      if(this.ColorizationThread != null){
        if(this.ColorizationThread.IsAlive == true){
          this.ColorizationThread.Abort();
        }
      }
    }
    private void StopColorization(System.Object sender, System.EventArgs E){
      this.ColorizationThread.Abort();
      this.StartStopToolStripMenuItem.Click -= this.StopColorization;
      this.StartStopToolStripMenuItem.Click += this.StartColorization;
      this.StartStopToolStripMenuItem.Text = "Start";
      this.OpenImageToolStripMenuItem.Enabled = true;
      this.MainProgressBar.Value = 0;
      this.Text = "PABCSoft - FastNeuralColor: Interrupted";
    }
    private unsafe void Colorize(){
      var Timer = System.DateTime.Now;
      this.Text = "PABCSoft - FastNeuralColor: Executing";
      this.StartStopToolStripMenuItem.Text = "Stop";
      this.OpenImageToolStripMenuItem.Enabled = false;
      this.SaveImageToolStripMenuItem.Enabled = false;
      this.StartStopToolStripMenuItem.Click -= this.StartColorization;
      this.StartStopToolStripMenuItem.Click += this.StopColorization;
      var inp = new System.Drawing.Bitmap(720, 560);
      var Tmp = new System.Drawing.Bitmap(Application.InputImage, 640, 480);
      System.Drawing.Graphics.FromImage(inp).DrawImage(Methods.RestoreColors(Tmp), 40, 40);
      var BD = inp.LockBits(new System.Drawing.Rectangle(0, 0, 720, 560), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      var Rand = new System.Random();
      for(System.Int32 y = 0; y < 40; y++){
        var Addr = BD.Scan0.ToInt32() + y * BD.Stride;
        for(System.Int32 x = 0; x < 720; x++){
          var L = Rand.Next(0, 255);
          *(System.Byte*)(Addr + x * 3) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 1) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 2) = (System.Byte)L;
        }
      }
      for(System.Int32 y = 520; y < 560; y++){
        var Addr = BD.Scan0.ToInt32() + y * BD.Stride;
        for(System.Int32 x = 0; x < 720; x++){
          var L = Rand.Next(0, 255);
          *(System.Byte*)(Addr + x * 3) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 1) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 2) = (System.Byte)L;
        }
      }
      for(System.Int32 y = 0; y < 560; y++){
        var Addr = BD.Scan0.ToInt32() + y * BD.Stride;
        for(System.Int32 x = 680; x < 720; x++){
          var L = Rand.Next(0, 255);
          *(System.Byte*)(Addr + x * 3) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 1) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 2) = (System.Byte)L;
        }
      }
      for(System.Int32 y = 0; y < 560; y++){
        var Addr = BD.Scan0.ToInt32() + y * BD.Stride;
        for(System.Int32 x = 0; x < 40; x++){
          var L = Rand.Next(0, 255);
          *(System.Byte*)(Addr + x * 3) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 1) = (System.Byte)L;
          *(System.Byte*)(Addr + x * 3 + 2) = (System.Byte)L;
        }
      }
      inp.UnlockBits(BD);
      BD = null;
      Rand = null;
      var _Input = new System.Single[inp.Width * inp.Height * 3];
      var Width = inp.Width;
      var Height = inp.Height;
      BD = inp.LockBits(new System.Drawing.Rectangle(0, 0, Width, Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      var W = inp.Width - 1;
      var inpW = Width;
      System.Threading.Tasks.Parallel.For(0, Height, (System.Int32 y)=>{
        var Address = (System.Byte*)(BD.Scan0.ToInt32() + BD.Stride * y);
        for(System.Int32 x = 0; x < W + 1; x++){
          var a = (System.Single)(*Address);
          _Input[((inpW * y) + x) * 3 + 2] = a;
          Address += 1;
          a = (System.Single)(*Address);
          _Input[((inpW * y) + x) * 3 + 1] = a;
          Address += 1;
          a = (System.Single)(*Address);
          _Input[((inpW * y) + x) * 3] = a;
          Address += 1;
        }
      });
      inp.UnlockBits(BD);
      BD = null;
      Tmp = null;
      _Input = Methods.RGB2Y(_Input, Width, Height);
      this.MainProgressBar.Value += 7;
      //-> Conv1
      var Temp = Methods.Conv2D9x9(_Input, ModelData.Conv1_Filters, Width, Height, 1);
      Temp = Methods.instanceNorm(Temp, Width, Height, 32, ModelData.Conv1_Shift, ModelData.Conv1_Scale);
      System.GC.Collect();
      Temp = Methods.ReLU(Temp, Width, Height, 32);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> Conv2
      Temp = Methods.Conv2D3x3s2(Temp, ModelData.Conv2_Filters, Width, Height, 32);
      var _Width = System.Convert.ToInt32(Width / 2);
      var _Height = System.Convert.ToInt32(Height / 2);
      System.GC.Collect();
      Temp = Methods.instanceNorm(Temp, _Width, _Height, 64, ModelData.Conv2_Shift, ModelData.Conv2_Scale);
      System.GC.Collect();
      Temp = Methods.ReLU(Temp, _Width, _Height, 64);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> Conv3
      Temp = Methods.Conv2D3x3s2(Temp, ModelData.Conv3_Filters, _Width, _Height, 64);
      _Width = System.Convert.ToInt32(_Width / 2);
      _Height = System.Convert.ToInt32(_Height / 2);
      System.GC.Collect();
      Temp = Methods.instanceNorm(Temp, _Width, _Height, 128, ModelData.Conv3_Shift, ModelData.Conv3_Scale);
      System.GC.Collect();
      Temp = Methods.ReLU(Temp, _Width, _Height, 128);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> ResidualBlock1
      Temp = Methods.ResidualBlock(Temp, _Width, _Height, ModelData.Resid1_Conv1_Shift, ModelData.Resid1_Conv1_Scale, ModelData.Resid1_Conv1_Filters, ModelData.Resid1_Conv2_Shift, ModelData.Resid1_Conv2_Scale, ModelData.Resid1_Conv2_Filters);
      this.MainProgressBar.Value += 7;
      //-> ResidualBlock2
      Temp = Methods.ResidualBlock(Temp, _Width, _Height, ModelData.Resid2_Conv1_Shift, ModelData.Resid2_Conv1_Scale, ModelData.Resid2_Conv1_Filters, ModelData.Resid2_Conv2_Shift, ModelData.Resid2_Conv2_Scale, ModelData.Resid2_Conv2_Filters);
      this.MainProgressBar.Value += 7;
      //-> ResidualBlock3
      Temp = Methods.ResidualBlock(Temp, _Width, _Height, ModelData.Resid3_Conv1_Shift, ModelData.Resid3_Conv1_Scale, ModelData.Resid3_Conv1_Filters, ModelData.Resid3_Conv2_Shift, ModelData.Resid3_Conv2_Scale, ModelData.Resid3_Conv2_Filters);
      this.MainProgressBar.Value += 7;
      //-> ResidualBlock4
      Temp = Methods.ResidualBlock(Temp, _Width, _Height, ModelData.Resid4_Conv1_Shift, ModelData.Resid4_Conv1_Scale, ModelData.Resid4_Conv1_Filters, ModelData.Resid4_Conv2_Shift, ModelData.Resid4_Conv2_Scale, ModelData.Resid4_Conv2_Filters);
      this.MainProgressBar.Value += 7;
      //-> ResidualBlock5
      Temp = Methods.ResidualBlock(Temp, _Width, _Height, ModelData.Resid5_Conv1_Shift, ModelData.Resid5_Conv1_Scale, ModelData.Resid5_Conv1_Filters, ModelData.Resid5_Conv2_Shift, ModelData.Resid5_Conv2_Scale, ModelData.Resid5_Conv2_Filters);
      this.MainProgressBar.Value += 7;
      //-> TransposedConv1
      Temp = Methods.TransposedConv2D3x3(Temp, 2, ModelData.TConv1_Filters, 64, _Width, _Height, 128);
      _Width *= 2;
      _Height *= 2;
      System.GC.Collect();
      Temp = Methods.instanceNorm(Temp, _Width, _Height, 64, ModelData.TConv1_Shift, ModelData.TConv1_Scale);
      System.GC.Collect();
      Temp = Methods.ReLU(Temp, _Width, _Height, 64);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> TransposedConv2
      Temp = Methods.TransposedConv2D3x3(Temp, 2, ModelData.TConv2_Filters, 32, _Width, _Height, 64);
      _Width *= 2;
      _Height *= 2;
      System.GC.Collect();
      Temp = Methods.instanceNorm(Temp, _Width, _Height, 32, ModelData.TConv2_Shift, ModelData.TConv2_Scale);
      System.GC.Collect();
      Temp = Methods.ReLU(Temp, _Width, _Height, 32);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> (Transposed)Conv(3)4
      Temp = Methods.Conv2D9x9(Temp, ModelData.TConv3_Filters, _Width, _Height, 32);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> Tanh
      Temp = Methods.Tanh(Temp, _Width, _Height, 2);
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      //-> BitmapNormalization
      Temp = Methods.BuildRGB(Temp, _Input, _Width, _Height);
      System.GC.Collect();
      //-> Converting to Bitmap
      var Result = new System.Drawing.Bitmap(_Width, _Height);
      BD = Result.LockBits(new System.Drawing.Rectangle(0, 0, Result.Width, Result.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      var Scan0 = BD.Scan0.ToInt32();
      var Stride = BD.Stride;
      W = Result.Width - 1;
      System.Threading.Tasks.Parallel.For(0, (System.Int32)Result.Height, (System.Int32 y)=>{
        var Address = (System.Byte*)(Scan0 + y * Stride);
        for(System.Int32 x = 0; x < W + 1; x++){
          *Address = System.Convert.ToByte(Temp[((_Width * y) + x) * 3 + 2]);
          Address += 1;
          *Address = System.Convert.ToByte(Temp[((_Width * y) + x) * 3 + 1]);
          Address += 1;
          *Address = System.Convert.ToByte(Temp[((_Width * y) + x) * 3]);
          Address += 1;
        }
      });
      Result.UnlockBits(BD);
      BD = null;
      System.GC.Collect();
      this.MainProgressBar.Value += 7;
      Tmp = new System.Drawing.Bitmap(640, 480);
      System.Drawing.Graphics.FromImage(Tmp).DrawImage(Result, new System.Drawing.Rectangle(0, 0, 640, 480), new System.Drawing.Rectangle(40, 40, 640, 480), System.Drawing.GraphicsUnit.Pixel);
      Tmp = new System.Drawing.Bitmap(Tmp, Application.InputImage.Width, Application.InputImage.Height);
      Application.OutputImage = new System.Drawing.Bitmap(Application.InputImage);
      BD = Application.OutputImage.LockBits(new System.Drawing.Rectangle(0, 0, Application.InputImage.Width, Application.InputImage.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      var BD1 = Tmp.LockBits(new System.Drawing.Rectangle(0, 0, Tmp.Width, Tmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      Scan0 = BD.Scan0.ToInt32();
      Stride = BD.Stride;
      W = Tmp.Width - 1;
      var Scan0_1 = BD1.Scan0.ToInt32();
      var Stride_1 = BD1.Stride;
      System.Threading.Tasks.Parallel.For(0, Tmp.Height, (System.Int32 y)=>{
        var AddressStylized = (System.Byte*)(Scan0 + y * Stride);
        var AddressOrigin = (System.Byte*)(Scan0_1 + y * Stride_1);
        for(System.Int32 x = 0; x < W + 1; x++){
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
          var R = System.Convert.ToInt16(ContentY + 1.13983f * (OriginV - 128.0f));
          if (R > 255){
            R = 255;
          }
          else{
            if (R < 0){
              R = 0;
            }
          }
          var G = System.Convert.ToInt16(ContentY - 0.39465f * (OriginU - 128.0f) - 0.58060f * (OriginV - 128.0f));
          if (G > 255){
            G = 255;
          }
          else{
            if (G < 0){
              G = 0;
            }
          }
          var B = System.Convert.ToInt16(ContentY + 2.03211f * (OriginU - 128.0f));
          if (B > 255){
            B = 255;
          }
          else{
            if (B < 0){
              B = 0;
            }
          }
          *AddressStylized = (System.Byte)B;
          AddressStylized += 1;
          *AddressStylized = (System.Byte)G;
          AddressStylized += 1;
          *AddressStylized = (System.Byte)R;
          AddressStylized += 1;
        };
      });
      Application.OutputImage.UnlockBits(BD);
      Tmp.UnlockBits(BD1);
      BD = null;
      BD1 = null;
      Application.OutputImage = Methods.RestoreColors(Application.OutputImage);
      this.MainProgressBar.Value = 0;
      this.MainPictureBox.Image = Application.OutputImage;
      this.SaveImageToolStripMenuItem.Enabled = true;
      this.OpenImageToolStripMenuItem.Enabled = true;
      this.StartStopToolStripMenuItem.Text = "Start";
      this.StartStopToolStripMenuItem.Click -= this.StopColorization;
      this.StartStopToolStripMenuItem.Click += this.StartColorization;
      this.Text = "PABCSoft - FastNeuralColor: Completed in "+(System.DateTime.Now-Timer).TotalSeconds.ToString()+" seconds!";
      System.GC.Collect();
    }
    private void StartColorization(System.Object sender, System.EventArgs E){
      this.ColorizationThread = new System.Threading.Thread(this.Colorize);
      this.ColorizationThread.Start();
    }
    public MainForm(){
      this.ModelData = new Data(Decompress(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("FastNeuralColor.PABCModel")));
      System.GC.Collect();
      //-> MainForm
      this.ClientSize = new System.Drawing.Size(720, 480);
      this.Text = "PABCSoft - FastNeuralColor (C# Edition)";
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = System.Drawing.Icon.FromHandle((new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MainIcon.jpg"))).GetHicon());
      this.Closing += this.CloseSender;
      //-> MainToolStrip
      this.MainToolStrip = new System.Windows.Forms.ToolStrip();
      //-> ImageToolStripMenuItem
      this.ImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Image", new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Image.jpg")));
      //-> OpenImageToolStripMenuItem
      this.OpenImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Open", new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Open.png")), this.OpenImage);
      this.ImageToolStripMenuItem.DropDownItems.Add(this.OpenImageToolStripMenuItem);
      //-> SaveImageToolStripMenuItem
      this.SaveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem("Save", new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Save.png")), this.SaveImage);
      this.SaveImageToolStripMenuItem.Enabled = false;
      this.ImageToolStripMenuItem.DropDownItems.Add(this.SaveImageToolStripMenuItem);
      this.MainToolStrip.Items.Add(this.ImageToolStripMenuItem);
      //-> StartStopToolStripMenuItem
      this.StartStopToolStripMenuItem = new ToolStripMenuItem("Start", new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Start.png")), this.StartColorization);
      this.StartStopToolStripMenuItem.Enabled = false;
      this.MainToolStrip.Items.Add(this.StartStopToolStripMenuItem);
      //-> ExitToolStripMenuItem
      this.ExitToolStripMenuItem = new ToolStripMenuItem("Exit", new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Exit.png")), this.Close);
      this.MainToolStrip.Items.Add(this.ExitToolStripMenuItem);
      this.Controls.Add(this.MainToolStrip);
      //-> MainPictureBox
      this.MainPictureBox = new System.Windows.Forms.PictureBox();
      this.BackColor = System.Drawing.Color.White;
      this.MainPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.MainPictureBox.Top = this.MainToolStrip.Height + 10;
      this.MainPictureBox.Left = 10;
      this.MainPictureBox.Width = 700;
      this.MainPictureBox.Height = 480 - this.MainToolStrip.Height - 50;
      this.MainPictureBox.Image = new System.Drawing.Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PABCSoft Logo.jpg"));
      this.Controls.Add(this.MainPictureBox);
      //-> MainProgressBar
      this.MainProgressBar = new System.Windows.Forms.ProgressBar();
      this.MainProgressBar.Top = 450;
      this.MainProgressBar.Left = 10;
      this.MainProgressBar.Height = 20;
      this.MainProgressBar.Width = 700;
      this.Controls.Add(this.MainProgressBar);
    }
  }
  static class Application{
    public static System.Drawing.Bitmap InputImage;
    public static System.Drawing.Bitmap OutputImage;
    public const System.UInt32 OriginalModelSize = 6679424;
    [STAThread]
    public static void Main(){
      System.Windows.Forms.Application.EnableVisualStyles();
      System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
      System.Windows.Forms.Application.Run(new MainForm());
    }
  }
}