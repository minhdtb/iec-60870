using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeSelectAndCallQualifier : InformationElement
    {
        private readonly int action;
        private readonly int notice;

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
            return "Select and call qualifier, action: " + action + ", notice: " + notice;
        }
    }
}