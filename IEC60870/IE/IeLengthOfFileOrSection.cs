using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeLengthOfFileOrSection : InformationElement
    {
        private int value;

        public IeLengthOfFileOrSection(int value)
        {
            this.value = value;
        }

        public IeLengthOfFileOrSection(BinaryReader reader)
        {
            value = reader.ReadByte() | (reader.ReadByte() << 8) | (reader.ReadByte() << 16);
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte)value;
            buffer[i++] = (byte)(value >> 8);
            buffer[i] = (byte)(value >> 16);

            return 3;
        }

        public int getValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "Length of file or section: " + value;
        }
    }
}
