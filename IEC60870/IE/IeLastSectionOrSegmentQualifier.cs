using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeLastSectionOrSegmentQualifier : InformationElement
    {
        private readonly int value;

        public IeLastSectionOrSegmentQualifier(int value)
        {
            this.value = value;
        }

        public IeLastSectionOrSegmentQualifier(BinaryReader reader)
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
            return "Last section or segment qualifier: " + value;
        }
    }
}