using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeNameOfSection : InformationElement
    {
        private int value;

        public IeNameOfSection(int value)
        {
            this.value = value;
        }

        IeNameOfSection(BinaryReader reader)
        {
            value = reader.ReadByte();
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;

            return 1;
        }

        public int getValue()
        {
            return value;
        }

        public override String ToString()
        {
            return "Name of section: " + value;
        }
    }
}
