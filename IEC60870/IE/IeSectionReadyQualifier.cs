using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSectionReadyQualifier : InformationElement
    {
        private int value;
        private Boolean sectionNotReady;

        public IeSectionReadyQualifier(int value, Boolean sectionNotReady)
        {
            this.value = value;
            this.sectionNotReady = sectionNotReady;
        }

        public IeSectionReadyQualifier(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            value = b1 & 0x7f;
            sectionNotReady = ((b1 & 0x80) == 0x80);
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;
            if (sectionNotReady)
            {
                buffer[i] |= 0x80;
            }
            return 1;
        }

        public int getValue()
        {
            return value;
        }

        public Boolean isSectionNotReady()
        {
            return sectionNotReady;
        }

        public override String ToString()
        {
            return "Section ready qualifier: " + value + ", section not ready: " + sectionNotReady;
        }
    }
}
