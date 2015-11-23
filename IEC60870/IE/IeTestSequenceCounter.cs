using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeTestSequenceCounter : InformationElement
    {
        private int value;

        public IeTestSequenceCounter(int value)
        {
            if (value < 0 || value > 65535)
            {
                throw new ArgumentException("Value has to be in the range 0..65535");
            }
            this.value = value;
        }

        IeTestSequenceCounter(BinaryReader reader)
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
            return "Test sequence counter: " + value;
        }
    }
}
