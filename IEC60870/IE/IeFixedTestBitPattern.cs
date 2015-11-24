using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeFixedTestBitPattern : InformationElement
    {
        public IeFixedTestBitPattern()
        {
        }

        public IeFixedTestBitPattern(BinaryReader reader)
        {
            if (reader.ReadByte() != 0x55 || reader.ReadByte() != 0xaa)
            {
                throw new IOException("Incorrect bit pattern in Fixed Test Bit Pattern.");
            }
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i++] = 0x55;
            buffer[i] = (byte)0xaa;
            return 2;
        }

        public override String ToString()
        {
            return "Fixed test bit pattern";
        }
    }
}
