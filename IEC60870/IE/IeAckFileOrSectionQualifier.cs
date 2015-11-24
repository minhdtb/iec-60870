using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeAckFileOrSectionQualifier : InformationElement
    {
        private int action;
        private int notice;

        public IeAckFileOrSectionQualifier(int action, int notice)
        {
            this.action = action;
            this.notice = notice;
        }

        public IeAckFileOrSectionQualifier(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            action = b1 & 0x0f;
            notice = (b1 >> 4) & 0x0f;
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)(action | (notice << 4));
            return 1;
        }

        public int getRequest()
        {
            return action;
        }

        public int getFreeze()
        {
            return notice;
        }

        public override String ToString()
        {
            return "Acknowledge file or section qualifier, action: " + action + ", notice: " + notice;
        }
    }
}
