using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeFileSegment : InformationElement
    {
        private readonly int length;
        private readonly int offset;
        private readonly byte[] segment;

        public IeFileSegment(byte[] segment, int offset, int length)
        {
            this.segment = segment;
            this.offset = offset;
            this.length = length;
        }

        public IeFileSegment(BinaryReader reader)
        {
            length = reader.ReadByte();
            segment = reader.ReadBytes(length);
            offset = 0;
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte) length;

            Array.Copy(segment, offset, buffer, i, length);

            return length + 1;
        }

        public byte[] GetSegment()
        {
            return segment;
        }

        public override string ToString()
        {
            return "File segment of length: " + length;
        }
    }
}