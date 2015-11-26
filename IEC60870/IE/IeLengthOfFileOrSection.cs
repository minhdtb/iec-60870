using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeLengthOfFileOrSection : InformationElement
    {
        private readonly int value;

        public IeLengthOfFileOrSection(int value)
        {
            this.value = value;
        }

        public IeLengthOfFileOrSection(BinaryReader reader)
        {
            value = reader.ReadByte() | (reader.ReadByte() << 8) | (reader.ReadByte() << 16);
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte) value;
            buffer[i++] = (byte) (value >> 8);
            buffer[i] = (byte) (value >> 16);

            return 3;
        }

        public int GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "Length of file or section: " + value;
        }
    }
}