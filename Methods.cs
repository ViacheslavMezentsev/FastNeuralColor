using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace FastNeuralColor
{
    public static unsafe class Methods
    {
        public static Bitmap RestoreColors( Bitmap bmp )
        {
            var result = new Bitmap( bmp.Width, bmp.Height );

            var arrRed = new ushort[ 256 ];
            var arrGreen = new ushort[ 256 ];
            var arrBlue = new ushort[ 256 ];

            var Width = bmp.Width;
            var Height = bmp.Height;

            var BD = bmp.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            Parallel.For( 0, Height, y =>            
            {
                var address = ( byte* ) ( BD.Scan0.ToInt64() + y * BD.Stride );

                for ( var x = 0; x < Width; x++ )
                {
                    lock ( arrBlue )
                    {
                        arrBlue[ *address ] += 1;
                    }
                    address += 1;
                    lock ( arrGreen )
                    {
                        arrGreen[ *address ] += 1;
                    }
                    address += 1;
                    lock ( arrRed )
                    {
                        arrRed[ *address ] += 1;
                    }
                    address += 1;
                }
            } );

            Double q = Width * Height * 0.01;
            UInt16 newMinR = 0;
            UInt16 newMaxR = 0;
            UInt16 newMinG = 0;
            UInt16 newMaxG = 0;
            UInt16 newMinB = 0;
            UInt16 newMaxB = 0;
            UInt64 s = 0;
            for ( Byte i = 0; i <= 255; i++ )
            {
                s += arrRed[ i ];
                if ( s >= q )
                {
                    newMinR = i;
                    break;
                }
            }
            s = 0;
            for ( Byte i = 255; i >= 0; i-- )
            {
                s += arrRed[ i ];
                if ( s >= q )
                {
                    newMaxR = i;
                    break;
                }
            }
            s = 0;
            for ( Byte i = 0; i <= 255; i++ )
            {
                s += arrGreen[ i ];
                if ( s >= q )
                {
                    newMinG = i;
                    break;
                }
            }
            s = 0;
            for ( Byte i = 255; i >= 0; i-- )
            {
                s += arrGreen[ i ];
                if ( s >= q )
                {
                    newMaxG = i;
                    break;
                }
            }
            s = 0;
            for ( Byte i = 0; i <= 255; i++ )
            {
                s += arrBlue[ i ];
                if ( s >= q )
                {
                    newMinB = i;
                    break;
                }
            }
            s = 0;
            for ( Byte i = 255; i >= 0; i-- )
            {
                s += arrBlue[ i ];
                if ( s >= q )
                {
                    newMaxB = i;
                    break;
                }
            }
            var BW_BD = result.LockBits( new Rectangle( 0, 0, Width, Height ), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb );
            Parallel.For( 0, Height, y =>
            {
                Byte* BW_Address;
                Byte* In_Address;
                Int16 newR = 0;
                Int16 newG = 0;
                Int16 newB = 0;
                In_Address = ( Byte* ) ( BD.Scan0.ToInt64() + y * BD.Stride );
                BW_Address = ( Byte* ) ( BW_BD.Scan0.ToInt64() + y * BW_BD.Stride );
                for ( Int32 x = 0; x < Width; x++ )
                {
                    newB = Convert.ToInt16( Math.Round( (*In_Address - newMinB) * 255.0f / ( newMaxB - newMinB + 1 ) ) );
                    if ( newB < 0 )
                    {
                        newB = 0;
                    }
                    if ( newB > 255 )
                    {
                        newB = 255;
                    }
                    In_Address += 1;
                    newG = Convert.ToInt16( Math.Round( (*In_Address - newMinG) * 255.0f / ( newMaxG - newMinG + 1 ) ) );
                    if ( newG < 0 )
                    {
                        newG = 0;
                    }
                    if ( newG > 255 )
                    {
                        newG = 255;
                    }
                    In_Address += 1;
                    newR = Convert.ToInt16( Math.Round( (*In_Address - newMinR) * 255.0f / ( newMaxR - newMinR + 1 ) ) );
                    if ( newR < 0 )
                    {
                        newR = 0;
                    }
                    if ( newR > 255 )
                    {
                        newR = 255;
                    }
                    In_Address += 1;
                    *BW_Address = ( Byte ) newB;
                    BW_Address += 1;
                    *BW_Address = ( Byte ) newG;
                    BW_Address += 1;
                    *BW_Address = ( Byte ) newR;
                    BW_Address += 1;
                }
            } );
            bmp.UnlockBits( BD );
            result.UnlockBits( BW_BD );
            arrRed = null;
            arrGreen = null;
            arrBlue = null;
            GC.Collect();
            return result;
        }

        public static float[] RGB2Y( float[] RGB, int W, int H )
        {
            var result = new float[ W * H ];

            Parallel.For( 0, H, y =>
            {
                for ( var x = 0; x < W; x++ )
                {
                    var r = RGB[ ( W * y + x ) * 3 ];
                    var g = RGB[ ( W * y + x ) * 3 + 1 ];
                    var b = RGB[ ( W * y + x ) * 3 + 2 ];

                    result[ W * y + x ] = ( r * 0.299f + g * 0.587f + b * 0.114f ) / 255.0f - 0.5f;
                }
            } );

            return result;
        }

        public static float[] ReLU( float[] input, int w, int h, int d )
        {
            var temp = new float[ w * h * d ];

            Parallel.For( 0, d, _d =>
            {
                for ( var _y = 0; _y < h; _y++ )
                {
                    for ( var _x = 0; _x < w; _x++ )
                    {
                        var v = input[ ( w * _y + _x ) * d + _d ];

                        temp[ ( w * _y + _x ) * d + _d ] = v < 0 ? 0f : v;
                    }
                }
            } );

            return temp;
        }

        public static Single[] Conv2D3x3s1( Single[] input, Single[][] Filters, Int32 InputWidth, Int32 InputHeight, Int32 InputDepth )
        {
            var OutputWidth = Convert.ToInt32( ( InputWidth + 2 - 3 ) + 1 );
            var OutputHeight = Convert.ToInt32( ( InputHeight + 2 - 3 ) + 1 );
            var OutputDepth = Filters.Length;
            var tempOutput = new Single[ OutputWidth * OutputHeight * OutputDepth ];
            var xy_stride = 1;
            Parallel.For( 0, OutputDepth, d =>
            {
                var f = Filters[ d ];
                for ( Int32 ay = 0; ay < OutputHeight; ay++ )
                {
                    var y = ( -1 | 0 ) + ( xy_stride * ay );
                    for ( Int32 ax = 0; ax < OutputWidth; ax++ )
                    {
                        var x = ( -1 | 0 ) + ( xy_stride * ax );
                        var a = 0.0f;
                        for ( Byte fy = 0; fy < 3; fy++ )
                        {
                            var oy = y + fy;
                            for ( Byte fx = 0; fx < 3; fx++ )
                            {
                                var ox = x + fx;
                                if ( ( oy >= 0 ) && ( oy < InputHeight ) && ( ox >= 0 ) && ( ox < InputWidth ) )
                                {
                                    for ( Byte fd = 0; fd < InputDepth; fd++ )
                                    {
                                        a += f[ ( ( 3 * fy ) + fx ) * InputDepth + fd ] * input[ ( ( InputWidth * oy ) + ox ) * InputDepth + fd ];
                                    }
                                }
                            }
                        }

                        tempOutput[ ( ( OutputWidth * ay ) + ax ) * OutputDepth + d ] = a;
                    }
                }
            } );

            return tempOutput;
        }
        public static Single[] Conv2D3x3s2( Single[] input, Single[][] Filters, Int32 InputWidth, Int32 InputHeight, Int32 InputDepth )
        {
            var OutputWidth = Convert.ToInt32( ( InputWidth + 2 - 3 ) / 2 + 1 );
            var OutputHeight = Convert.ToInt32( ( InputHeight + 2 - 3 ) / 2 + 1 );
            var OutputDepth = Filters.Length;
            var tempOutput = new Single[ OutputWidth * OutputHeight * OutputDepth ];
            var xy_stride = 2;
            Parallel.For( 0, OutputDepth, d =>
            {
                var f = Filters[ d ];
                for ( Int32 ay = 0; ay < OutputHeight; ay++ )
                {
                    var y = ( -1 | 0 ) + ( xy_stride * ay );
                    for ( Int32 ax = 0; ax < OutputWidth; ax++ )
                    {
                        var x = ( -1 | 0 ) + ( xy_stride * ax );
                        var a = 0.0f;
                        for ( Byte fy = 0; fy < 3; fy++ )
                        {
                            var oy = y + fy;
                            for ( Byte fx = 0; fx < 3; fx++ )
                            {
                                var ox = x + fx;
                                if ( ( oy >= 0 ) && ( oy < InputHeight ) && ( ox >= 0 ) && ( ox < InputWidth ) )
                                {
                                    for ( Byte fd = 0; fd < InputDepth; fd++ )
                                    {
                                        a += f[ ( ( 3 * fy ) + fx ) * InputDepth + fd ] * input[ ( ( InputWidth * oy ) + ox ) * InputDepth + fd ];
                                    }
                                }
                            }
                        }
                        tempOutput[ ( ( OutputWidth * ay ) + ax ) * OutputDepth + d ] = a;
                    }
                }
            } );
            return tempOutput;
        }
        public static Single[] Conv2D9x9( Single[] input, Single[][] Filters, Int32 InputWidth, Int32 InputHeight, Int32 InputDepth )
        {
            var OutputWidth = Convert.ToInt32( ( InputWidth + 8 - 9 ) + 1 );
            var OutputHeight = Convert.ToInt32( ( InputHeight + 8 - 9 ) + 1 );
            var OutputDepth = Filters.Length;
            var tempOutput = new Single[ OutputWidth * OutputHeight * OutputDepth ];
            var xy_stride = 1;
            Parallel.For( 0, OutputDepth, d =>
            {
                var f = Filters[ d ];
                for ( Int32 ay = 0; ay < OutputHeight; ay++ )
                {
                    var y = ( -4 | 0 ) + ( xy_stride * ay );
                    for ( Int32 ax = 0; ax < OutputWidth; ax++ )
                    {
                        var x = ( -4 | 0 ) + ( xy_stride * ax );
                        var a = 0.0f;
                        for ( Byte fy = 0; fy < 9; fy++ )
                        {
                            var oy = y + fy;
                            for ( Byte fx = 0; fx < 9; fx++ )
                            {
                                var ox = x + fx;
                                if ( ( oy >= 0 ) && ( oy < InputHeight ) && ( ox >= 0 ) && ( ox < InputWidth ) )
                                {
                                    for ( Byte fd = 0; fd < InputDepth; fd++ )
                                    {
                                        a += f[ ( ( 9 * fy ) + fx ) * InputDepth + fd ] * input[ ( ( InputWidth * oy ) + ox ) * InputDepth + fd ];
                                    }
                                }
                            }
                        }
                        tempOutput[ ( ( OutputWidth * ay ) + ax ) * OutputDepth + d ] = a;
                    }
                }
            } );
            return tempOutput;
        }
        public static Single[] TransposedConv2D3x3( Single[] input, Int32 Stride, Single[][] Filters, Int32 FilterDepth, Int32 InputWidth, Int32 InputHeight, Int32 InputDepth )
        {
            var OutputWidth = InputWidth * Stride;
            var OutputHeight = InputHeight * Stride;
            var OutputDepth = FilterDepth;
            var tempOutput = new Single[ OutputWidth * OutputHeight * OutputDepth ];
            for ( Int32 y = 0; y < InputHeight; y++ )
            {
                for ( Int32 x = 0; x < InputWidth; x++ )
                {
                    for ( Int32 d = 0; d < Filters.Length; d++ )
                    {
                        var Filter = Filters[ d ];
                        for ( Byte fd = 0; fd < FilterDepth; fd++ )
                        {
                            for ( Byte fy = 0; fy < 3; fy++ )
                            {
                                for ( Byte fx = 0; fx < 3; fx++ )
                                {
                                    var ox = x * Stride + fx;
                                    var oy = y * Stride + fy;
                                    if ( ( ox < OutputWidth ) && ( oy < OutputHeight ) )
                                    {
                                        tempOutput[ ( ( OutputWidth * oy ) + ox ) * OutputDepth + fd ] = input[ ( ( InputWidth * y ) + x ) * InputDepth + d ] * Filter[ ( ( 3 * fy ) + fx ) * FilterDepth + fd ] + tempOutput[ ( ( OutputWidth * oy ) + ox ) * OutputDepth + fd ];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tempOutput;
        }
        public static Single[] instanceNorm( Single[] input, Int32 InputWidth, Int32 InputHeight, Int32 InputDepth, Single[] Shift, Single[] Scale )
        {
            var _Mean = new Single[ InputDepth ];
            var Variance = new Single[ InputDepth ];
            for ( Int32 d = 0; d < InputDepth; d++ )
            {
                Double Temp = 0.0;
                for ( Int32 y = 0; y < InputHeight; y++ )
                {
                    for ( Int32 x = 0; x < InputWidth; x++ )
                    {
                        Temp += input[ ( ( InputWidth * y ) + x ) * InputDepth + d ];
                    }
                }
                Temp /= ( Double ) InputHeight * InputWidth;
                _Mean[ d ] = ( Single ) Temp;
                Double Temp2 = 0.0;
                for ( Int32 y = 0; y < InputHeight; y++ )
                {
                    for ( Int32 x = 0; x < InputWidth; x++ )
                    {
                        var _x = input[ ( ( InputWidth * y ) + x ) * InputDepth + d ] - Temp;
                        Temp2 += _x * _x;
                    }
                }
                Temp2 /= ( Double ) InputHeight * InputWidth;
                Variance[ d ] = ( Single ) Temp2;
            }
            var _Temp = new Single[ InputWidth * InputHeight * InputDepth ];
            var inp = input;
            Parallel.For( 0, InputDepth, d =>
            {
                for ( Int32 y = 0; y < InputHeight; y++ )
                {
                    for ( Int32 x = 0; x < InputWidth; x++ )
                    {
                        _Temp[ ( ( InputWidth * y ) + x ) * InputDepth + d ] = ( Single ) ( ( ( inp[ ( ( InputWidth * y ) + x ) * InputDepth + d ] - _Mean[ d ] ) / Math.Sqrt( 0.001f + Variance[ d ] ) ) * Scale[ d ] + Shift[ d ] );
                    }
                }
            } );
            return _Temp;
        }
        public static Single[] ResidualBlock( Single[] input, Int32 InputWidth, Int32 InputHeight, Single[] Shift1, Single[] Scale1, Single[][] Filters1, Single[] Shift2, Single[] Scale2, Single[][] Filters2 )
        {
            var Conv1 = Conv2D3x3s1( input, Filters1, InputWidth, InputHeight, 128 );
            Conv1 = instanceNorm( Conv1, InputWidth, InputHeight, 128, Shift1, Scale1 );
            Conv1 = ReLU( Conv1, InputWidth, InputHeight, 128 );
            var Conv2 = Conv2D3x3s1( Conv1, Filters2, InputWidth, InputHeight, 128 );
            Conv2 = instanceNorm( Conv2, InputWidth, InputHeight, 128, Shift2, Scale2 );
            var Temp = new Single[ InputWidth * InputHeight * 128 ];
            var inp = input;
            Parallel.For( 0, 128, d =>
            {
                for ( Int32 y = 0; y < InputHeight; y++ )
                {
                    for ( Int32 x = 0; x < InputWidth; x++ )
                    {
                        Temp[ ( ( InputWidth * y ) + x ) * 128 + d ] = inp[ ( ( InputWidth * y ) + x ) * 128 + d ] + Conv2[ ( ( InputWidth * y ) + x ) * 128 + d ];
                    }
                }
            } );
            Conv1 = null;
            Conv2 = null;
            GC.Collect();
            return Temp;
        }
        public static Single[] Tanh( Single[] input, Int32 InputWidth, Int32 InputHeight, Int32 InputDepth )
        {
            var Temp = new Single[ InputWidth * InputHeight * InputDepth ];
            var inp = input;
            Parallel.For( 0, InputDepth, d =>
            {
                for ( Int32 y = 0; y < InputHeight; y++ )
                {
                    for ( Int32 x = 0; x < InputWidth; x++ )
                    {
                        Temp[ ( ( InputWidth * y ) + x ) * InputDepth + d ] = ( Single ) Math.Tanh( inp[ ( ( InputWidth * y ) + x ) * InputDepth + d ] );
                    }
                }
            } );
            return Temp;
        }
        public static Single[] BuildRGB( Single[] inputUV, Single[] inputY, Int32 InputWidth, Int32 InputHeight )
        {
            var Result = new Single[ InputWidth * InputHeight * 3 ];
            var min = 99999999.0f;
            var max = 0.0f;
            for ( UInt16 x = 0; x < InputWidth; x++ )
            {
                for ( UInt16 y = 0; y < InputHeight; y++ )
                {
                    var v = inputUV[ ( ( InputWidth * y ) + x ) * 2 + 0 ];
                    if ( v < min )
                    {
                        min = v;
                    }
                    if ( v > max )
                    {
                        max = v;
                    }
                    v = inputUV[ ( ( InputWidth * y ) + x ) * 2 + 1 ];
                    if ( v < min )
                    {
                        min = v;
                    }
                    if ( v > max )
                    {
                        max = v;
                    }
                }
            }
            for ( Int32 x = 0; x < InputWidth; x++ )
            {
                for ( Int32 y = 0; y < InputHeight; y++ )
                {
                    Single _Y = ( inputY[ ( ( InputWidth * y ) + x ) ] + 0.5f ) * 255.0f;
                    Single _U = ( ( inputUV[ ( ( InputWidth * y ) + x ) * 2 + 0 ] * 0.436f - min ) / ( max - min ) ) * 239.17f;
                    Single _V = ( ( inputUV[ ( ( InputWidth * y ) + x ) * 2 + 1 ] * 0.615f - min ) / ( max - min ) ) * 313.65f - 28.8f;
                    Single r = _Y + ( 1.4075f * ( _V - 128.0f ) );
                    Single g = _Y - ( 0.3455f * ( _U - 128.0f ) - ( 0.7169f * ( _V - 128.0f ) ) );
                    Single b = _Y + ( 1.7790f * ( _U - 128.0f ) );
                    if ( r < 0.0f )
                    {
                        r = 0.0f;
                    }
                    if ( r > 255.0f )
                    {
                        r = 255.0f;
                    }
                    if ( g < 0.0f )
                    {
                        g = 0.0f;
                    }
                    if ( g > 255.0f )
                    {
                        g = 255.0f;
                    }
                    if ( b < 0.0f )
                    {
                        b = 0.0f;
                    }
                    if ( b > 255.0f )
                    {
                        b = 255.0f;
                    }
                    Result[ ( ( InputWidth * y ) + x ) * 3 + 0 ] = r;
                    Result[ ( ( InputWidth * y ) + x ) * 3 + 1 ] = g;
                    Result[ ( ( InputWidth * y ) + x ) * 3 + 2 ] = b;
                }
            }
            return Result;
        }
    }
}
