using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeNameOfFile : InformationElement
    {
        private int value;

        public IeNameOfFile(int value)
        {
            this.value = value;
        }

        public IeNameOfFile(BinaryReader reader)
        {
            value = reader.ReadByte() | (reader.ReadByte() << 8);
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte)value;
            buffer[i] = (byte)(value >> 8);

            return 2;
        }

        public int getValue()
        {
            return value;
        }

        public override String ToString()
        {
            return "Name of file: " + value;
        }
    }
}
