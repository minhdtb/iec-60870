using IEC60870.IE.Base;
using System;
using System.IO;
using System.Text;

namespace IEC60870.IE
{
    public class IeBinaryStateInformation : InformationElement
    {
        private int value;

        public IeBinaryStateInformation(int value)
        {
            this.value = value;
        }

        public IeBinaryStateInformation(BinaryReader reader)
        {
            value = reader.ReadInt32();
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte)(value >> 24);
            buffer[i++] = (byte)(value >> 16);
            buffer[i++] = (byte)(value >> 8);
            buffer[i] = (byte)value;
            return 4;
        }

        public int getValue()
        {
            return value;
        }

        public Boolean getBinaryState(int position)
        {
            if (position < 1 || position > 32)
            {
                throw new ArgumentException("Position out of bound. Should be between 1 and 32.");
            }

            return (((value >> (position - 1)) & 0x01) == 0x01);
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(value.ToString("X"));
            while (sb.Length < 8)
            {
                sb.Insert(0, '0'); // pad with leading zero if needed
            }

            return "BinaryStateInformation (first bit = LSB): " + sb.ToString();
        }
    }
}
