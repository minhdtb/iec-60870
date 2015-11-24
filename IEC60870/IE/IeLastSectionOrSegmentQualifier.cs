using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    class IeLastSectionOrSegmentQualifier : InformationElement
    {
        private int value;

        public IeLastSectionOrSegmentQualifier(int value)
        {
            this.value = value;
        }

        public IeLastSectionOrSegmentQualifier(BinaryReader reader)
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
            return "Last section or segment qualifier: " + value;
        }
    }
}
