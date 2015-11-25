using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQualifierOfInterrogation : InformationElement
    {
        private int value;

        public IeQualifierOfInterrogation(int value)
        {
            this.value = value;
        }

        public IeQualifierOfInterrogation(BinaryReader reader)
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

        public override string ToString()
        {
            return "Qualifier of interrogation: " + value;
        }
    }
}
