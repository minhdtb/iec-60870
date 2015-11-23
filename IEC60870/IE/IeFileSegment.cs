using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeFileSegment : InformationElement
    {
        private byte[] segment;
        private int offset;
        private int length;

        public IeFileSegment(byte[] segment, int offset, int length)
        {
            this.segment = segment;
            this.offset = offset;
            this.length = length;
        }

        IeFileSegment(BinaryReader reader)
        {
            length = reader.ReadByte();            
            segment = reader.ReadBytes(length);
            offset = 0;
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte)length;

            Array.Copy(segment, offset, buffer, i, length);

            return length + 1;
        }

        public byte[] getSegment()
        {
            return segment;
        }

        public override String ToString()
        {
            return "File segment of length: " + length;
        }
    }
}
