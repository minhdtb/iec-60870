using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeShortFloat : InformationElement
    {
        private float value;

        public IeShortFloat(float value)
        {
            this.value = value;
        }

        IeShortFloat(BinaryReader reader)
        {
            var data = reader.ReadByte() | (reader.ReadByte() << 8) | (reader.ReadByte() << 16) | (reader.ReadByte() << 24);
            byte[] bytes = BitConverter.GetBytes(data);
            value = BitConverter.ToSingle(bytes, 0);
        }

        public override int encode(byte[] buffer, int i)
        {
            int tempVal = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            buffer[i++] = (byte)tempVal;
            buffer[i++] = (byte)(tempVal >> 8);
            buffer[i++] = (byte)(tempVal >> 16);
            buffer[i] = (byte)(tempVal >> 24);

            return 4;
        }

        public float getValue()
        {
            return value;
        }

        public override String ToString()
        {
            return "Short float value: " + value;
        }
    }
}
