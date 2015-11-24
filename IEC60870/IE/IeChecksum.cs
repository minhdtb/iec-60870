using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeChecksum : InformationElement
    {
        private int value;

        public IeChecksum(int value)
        {
            this.value = value;
        }

        public IeChecksum(BinaryReader reader)
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
            return "Checksum: " + value;
        }
    }
}
