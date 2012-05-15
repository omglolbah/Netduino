using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoPlusApplication1
{
    class MatrixDriver
    {
        //74HC595 required!
        ShiftRegister Register;
        public MatrixDriver(ShiftRegister reg)
        {
            Register = reg;
        }

        int[] map = new int[8 * 8];

        Boolean SetPixel(int x, int y, int val)
        {
            map[y * 8 + x] = val;
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
            for (int i = 1; i <= 128; i = i * 2)
            {
                int rowdata = GetRow(i);
                Register.WriteByte(0, false);
                Register.WriteByte(0, true);

                Register.WriteByte(i, false);
                Register.WriteByte(rowdata, true);
            }
        }
        private int GetRow(int row)
        {
            int start = row;
            int val = 0;
            int pow = 1;
            for (int i = 0; i < 8; i++)
            {
                val += map[start + i];
                pow = pow * 2;
            }
            return val;
        }
    }
}
