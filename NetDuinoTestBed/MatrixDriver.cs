using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace NetduinoPlusApplication1
{
    public enum MatrixDimension
    {
        MatrixRow = 0,  //A row of segments
        MatrixBlock = 1 //A 2x2 block of segments
    }
    class MatrixDriver
    {
        //74HC595 required!
        ShiftRegister Register;
        MatrixDimension MatrixDim;
        int MatrixLength;
        int[] map;
        

        public MatrixDriver(ShiftRegister reg, MatrixDimension mat, int segments)
        {
            Register = reg;
            MatrixDim = mat;
            MatrixLength = segments*8;
            switch(MatrixDim){
                case MatrixDimension.MatrixRow:
                    map = new int[8 * (MatrixLength)]; // 4 segments in a row
                    break;
                case MatrixDimension.MatrixBlock:
                    map = new int[MatrixLength * MatrixLength];
                    break;
            }
        }
        
        Boolean SetPixel(int x, int y, int val)
        {
            //Preserved like this for future changes, even though code is both cases are identical
            switch(MatrixDim){
                case MatrixDimension.MatrixRow:
                    map[y * MatrixLength + x] = val;
                    break;
                case MatrixDimension.MatrixBlock:
                    map[y * MatrixLength + x] = val;
                    break;
            }
            
            Debug.Print("map[" + x + "," + y + "]=" + val);
            return true;
        }
        Boolean SetPixel(int x, int y)
        {
            return SetPixel(x, y, 1);
        }
        Boolean ClearPixel(int x, int y)
        {
            return SetPixel(x, y, 0);
        }
        void WriteData()
        {
            //Writes the map out to the display

            switch (MatrixDim)
            {
                case MatrixDimension.MatrixRow:
                    
                    break;
                case MatrixDimension.MatrixBlock:
                    break;
            }
        }
        public int Pow(int a, int b)
        {
            return (int) System.Math.Pow((double)a, (double)b);
        }
        private void WriteColumn(int c)
        {
            int max=0;
            switch(MatrixDim){
                case MatrixDimension.MatrixRow:
                    max = Pow(2,8);
                    break;
                case MatrixDimension.MatrixBlock:
                    max = Pow(2, MatrixLength);
                    break;
            }
            for (int i = 1; i < max; i++)
            {
                Register.WriteBit((i & c) > 0);
            }
        }
        private void WriteRow(int[] m, int r)
        {
            int start;
            int len;
            switch (MatrixDim)
            {
                case MatrixDimension.MatrixRow:
                    start = r * 8;
                    len = MatrixLength;
                    for (int i = start; i < len; i++)
                    {
                        Register.WriteBit(m[i] > 0);
                    }
                    break;
                case MatrixDimension.MatrixBlock:
                    start = r * MatrixLength;
                    len = MatrixLength;
                    for (int i = start; i < len; i++)
                    {
                        Register.WriteBit(m[i] > 0);
                    }
                    break;
            }
        }
    }
}
