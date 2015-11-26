using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeNormalizedValue : InformationElement
    {
        protected int Value;

        public IeNormalizedValue(int value)
        {
            if (value < -32768 || value > 32767)
            {
                throw new ArgumentException("Value has to be in the range -32768..32767");
            }
            Value = value;
        }

        public IeNormalizedValue(BinaryReader reader)
        {
            Value = reader.ReadByte() | (reader.ReadByte() << 8);
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte) Value;
            buffer[i] = (byte) (Value >> 8);

            return 2;
        }

        public int GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return "Normalized value: " + (double) Value/32768;
        }
    }
}