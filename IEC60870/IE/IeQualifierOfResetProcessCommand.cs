using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQualifierOfResetProcessCommand : InformationElement
    {
        private int value;

        public IeQualifierOfResetProcessCommand(int value)
        {
            this.value = value;
        }

        public IeQualifierOfResetProcessCommand(BinaryReader reader)
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
            return "Qualifier of reset process command: " + value;
        }
    }
}
