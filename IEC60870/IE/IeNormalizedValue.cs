using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeNormalizedValue : InformationElement
    {
        int value;

        public IeNormalizedValue(int value)
        {
            if (value < -32768 || value > 32767)
            {
                throw new ArgumentException("Value has to be in the range -32768..32767");
            }
            this.value = value;
        }

        IeNormalizedValue(BinaryReader reader)
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
            return "Normalized value: " + ((double)value / 32768);
        }
    }
}
