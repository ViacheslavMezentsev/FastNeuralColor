using System;
using System.IO;

namespace FastNeuralColor
{
    public sealed class Data
    {
        public float[][] Conv1_Filters;
        public float[] Conv1_Shift;
        public float[] Conv1_Scale;
        public float[][] Conv2_Filters;
        public float[] Conv2_Shift;
        public float[] Conv2_Scale;
        public float[][] Conv3_Filters;
        public float[] Conv3_Shift;
        public float[] Conv3_Scale;
        //-> Residual
        public float[][] Resid1_Conv1_Filters;
        public float[] Resid1_Conv1_Shift;
        public float[] Resid1_Conv1_Scale;
        public float[][] Resid1_Conv2_Filters;
        public float[] Resid1_Conv2_Shift;
        public float[] Resid1_Conv2_Scale;
        public float[][] Resid2_Conv1_Filters;
        public float[] Resid2_Conv1_Shift;
        public float[] Resid2_Conv1_Scale;
        public float[][] Resid2_Conv2_Filters;
        public float[] Resid2_Conv2_Shift;
        public float[] Resid2_Conv2_Scale;
        public float[][] Resid3_Conv1_Filters;
        public float[] Resid3_Conv1_Shift;
        public float[] Resid3_Conv1_Scale;
        public float[][] Resid3_Conv2_Filters;
        public float[] Resid3_Conv2_Shift;
        public float[] Resid3_Conv2_Scale;
        public float[][] Resid4_Conv1_Filters;
        public float[] Resid4_Conv1_Shift;
        public float[] Resid4_Conv1_Scale;
        public float[][] Resid4_Conv2_Filters;
        public float[] Resid4_Conv2_Shift;
        public float[] Resid4_Conv2_Scale;
        public float[][] Resid5_Conv1_Filters;
        public float[] Resid5_Conv1_Shift;
        public float[] Resid5_Conv1_Scale;
        public float[][] Resid5_Conv2_Filters;
        public float[] Resid5_Conv2_Shift;
        public float[] Resid5_Conv2_Scale;
        //-> Deconv
        public float[][] TConv1_Filters;
        public float[] TConv1_Shift;
        public float[] TConv1_Scale;
        public float[][] TConv2_Filters;
        public float[] TConv2_Shift;
        public float[] TConv2_Scale;
        public float[][] TConv3_Filters;
        private static float[][] ReadFilters( byte n, byte d, byte h, byte w, BinaryReader br )
        {
            var Result = new float[ n ][];
            for ( int _n = 0; _n < n; _n++ )
            {
                Result[ _n ] = new float[ w * h * d ];
            }
            for ( int _n = 0; _n < n; _n++ )
            {
                for ( int _d = 0; _d < d; _d++ )
                {
                    for ( int _h = 0; _h < h; _h++ )
                    {
                        for ( int _w = 0; _w < w; _w++ )
                        {
                            Result[ _n ][ ( ( w * _h ) + _w ) * d + _d ] = br.ReadSingle();
                        }
                    }
                }
            }
            return Result;
        }
        private static float[] ReadLinear( byte n, BinaryReader br )
        {
            var Result = new float[ n ];
            for ( int i = 0; i < n; i++ )
            {
                Result[ i ] = br.ReadSingle();
            }
            return Result;
        }
        public Data( Stream BaseStream )
        {
            var fs = new BinaryReader( BaseStream );
            Conv1_Filters = ReadFilters( 32, 1, 9, 9, fs );
            Conv1_Shift = ReadLinear( 32, fs );
            Conv1_Scale = ReadLinear( 32, fs );
            Conv2_Filters = ReadFilters( 64, 32, 3, 3, fs );
            Conv2_Shift = ReadLinear( 64, fs );
            Conv2_Scale = ReadLinear( 64, fs );
            Conv3_Filters = ReadFilters( 128, 64, 3, 3, fs );
            Conv3_Shift = ReadLinear( 128, fs );
            Conv3_Scale = ReadLinear( 128, fs );
            Resid1_Conv1_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid1_Conv1_Shift = ReadLinear( 128, fs );
            Resid1_Conv1_Scale = ReadLinear( 128, fs );
            Resid1_Conv2_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid1_Conv2_Shift = ReadLinear( 128, fs );
            Resid1_Conv2_Scale = ReadLinear( 128, fs );
            Resid2_Conv1_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid2_Conv1_Shift = ReadLinear( 128, fs );
            Resid2_Conv1_Scale = ReadLinear( 128, fs );
            Resid2_Conv2_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid2_Conv2_Shift = ReadLinear( 128, fs );
            Resid2_Conv2_Scale = ReadLinear( 128, fs );
            Resid3_Conv1_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid3_Conv1_Shift = ReadLinear( 128, fs );
            Resid3_Conv1_Scale = ReadLinear( 128, fs );
            Resid3_Conv2_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid3_Conv2_Shift = ReadLinear( 128, fs );
            Resid3_Conv2_Scale = ReadLinear( 128, fs );
            Resid4_Conv1_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid4_Conv1_Shift = ReadLinear( 128, fs );
            Resid4_Conv1_Scale = ReadLinear( 128, fs );
            Resid4_Conv2_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid4_Conv2_Shift = ReadLinear( 128, fs );
            Resid4_Conv2_Scale = ReadLinear( 128, fs );
            Resid5_Conv1_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid5_Conv1_Shift = ReadLinear( 128, fs );
            Resid5_Conv1_Scale = ReadLinear( 128, fs );
            Resid5_Conv2_Filters = ReadFilters( 128, 128, 3, 3, fs );
            Resid5_Conv2_Shift = ReadLinear( 128, fs );
            Resid5_Conv2_Scale = ReadLinear( 128, fs );
            TConv1_Filters = ReadFilters( 128, 64, 3, 3, fs );
            TConv1_Shift = ReadLinear( 64, fs );
            TConv1_Scale = ReadLinear( 64, fs );
            TConv2_Filters = ReadFilters( 64, 32, 3, 3, fs );
            TConv2_Shift = ReadLinear( 32, fs );
            TConv2_Scale = ReadLinear( 32, fs );
            TConv3_Filters = ReadFilters( 2, 32, 9, 9, fs );
            fs.Close();
        }
    }
}
