using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeNameOfFile : InformationElement
    {
        private readonly int value;

        public IeNameOfFile(int value)
        {
            this.value = value;
        }

        public IeNameOfFile(BinaryReader reader)
        {
            value = reader.ReadByte() | (reader.ReadByte() << 8);
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte) value;
            buffer[i] = (byte) (value >> 8);

            return 2;
        }

        public int GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "Name of file: " + value;
        }
    }
}