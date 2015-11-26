using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeQualifierOfResetProcessCommand : InformationElement
    {
        private readonly int value;

        public IeQualifierOfResetProcessCommand(int value)
        {
            this.value = value;
        }

        public IeQualifierOfResetProcessCommand(BinaryReader reader)
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
            return "Qualifier of reset process command: " + value;
        }
    }
}