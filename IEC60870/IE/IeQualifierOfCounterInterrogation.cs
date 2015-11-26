using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeQualifierOfCounterInterrogation : InformationElement
    {
        private readonly int freeze;
        private readonly int request;

        public IeQualifierOfCounterInterrogation(int request, int freeze)
        {
            this.request = request;
            this.freeze = freeze;
        }

        public IeQualifierOfCounterInterrogation(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            request = b1 & 0x3f;
            freeze = (b1 >> 6) & 0x03;
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) (request | (freeze << 6));
            return 1;
        }

        public int GetRequest()
        {
            return request;
        }

        public int GetFreeze()
        {
            return freeze;
        }

        public override string ToString()
        {
            return "Qualifier of counter interrogation, request: " + request + ", freeze: " + freeze;
        }
    }
}