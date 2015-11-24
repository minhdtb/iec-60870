using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSelectAndCallQualifier : InformationElement
    {
        private int action;
        private int notice;

        public IeSelectAndCallQualifier(int action, int notice)
        {
            this.action = action;
            this.notice = notice;
        }

        public IeSelectAndCallQualifier(BinaryReader reader)
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
            return "Select and call qualifier, action: " + action + ", notice: " + notice;
        }
    }
}
