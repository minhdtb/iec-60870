using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeSectionReadyQualifier : InformationElement
    {
        private readonly bool sectionNotReady;
        private readonly int value;

        public IeSectionReadyQualifier(int value, bool sectionNotReady)
        {
            this.value = value;
            this.sectionNotReady = sectionNotReady;
        }

        public IeSectionReadyQualifier(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            value = b1 & 0x7f;
            sectionNotReady = (b1 & 0x80) == 0x80;
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) value;
            if (sectionNotReady)
            {
                buffer[i] |= 0x80;
            }
            return 1;
        }

        public int GetValue()
        {
            return value;
        }

        public bool IsSectionNotReady()
        {
            return sectionNotReady;
        }

        public override string ToString()
        {
            return "Section ready qualifier: " + value + ", section not ready: " + sectionNotReady;
        }
    }
}