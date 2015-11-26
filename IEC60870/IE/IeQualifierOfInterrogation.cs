using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeQualifierOfInterrogation : InformationElement
    {
        private readonly int value;

        public IeQualifierOfInterrogation(int value)
        {
            this.value = value;
        }

        public IeQualifierOfInterrogation(BinaryReader reader)
        {
            value = reader.ReadByte();
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) value;
            return 1;
        }

        public int GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "Qualifier of interrogation: " + value;
        }
    }
}