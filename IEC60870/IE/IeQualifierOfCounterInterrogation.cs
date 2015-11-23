using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQualifierOfCounterInterrogation : InformationElement
    {
        private int request;
        private int freeze;

        public IeQualifierOfCounterInterrogation(int request, int freeze)
        {
            this.request = request;
            this.freeze = freeze;
        }

        IeQualifierOfCounterInterrogation(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            request = b1 & 0x3f;
            freeze = (b1 >> 6) & 0x03;
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)(request | (freeze << 6));
            return 1;
        }

        public int getRequest()
        {
            return request;
        }

        public int getFreeze()
        {
            return freeze;
        }

        public override String ToString()
        {
            return "Qualifier of counter interrogation, request: " + request + ", freeze: " + freeze;
        }
    }
}
