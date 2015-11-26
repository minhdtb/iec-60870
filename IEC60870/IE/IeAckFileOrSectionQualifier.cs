using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeAckFileOrSectionQualifier : InformationElement
    {
        private readonly int action;
        private readonly int notice;

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

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) (action | (notice << 4));
            return 1;
        }

        public int GetRequest()
        {
            return action;
        }

        public int GetFreeze()
        {
            return notice;
        }

        public override string ToString()
        {
            return "Acknowledge file or section qualifier, action: " + action + ", notice: " + notice;
        }
    }
}