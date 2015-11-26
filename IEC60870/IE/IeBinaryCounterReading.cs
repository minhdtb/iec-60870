using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeBinaryCounterReading : InformationElement
    {
        private readonly bool carry;
        private readonly bool counterAdjusted;
        private readonly int counterReading;
        private readonly bool invalid;
        private readonly int sequenceNumber;

        public IeBinaryCounterReading(int counterReading, int sequenceNumber, bool carry, bool counterAdjusted,
            bool invalid)
        {
            this.counterReading = counterReading;
            this.sequenceNumber = sequenceNumber;
            this.carry = carry;
            this.counterAdjusted = counterAdjusted;
            this.invalid = invalid;
        }

        public IeBinaryCounterReading(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            int b2 = reader.ReadByte();
            int b3 = reader.ReadByte();
            int b4 = reader.ReadByte();
            int b5 = reader.ReadByte();

            carry = (b5 & 0x20) == 0x20;
            counterAdjusted = (b5 & 0x40) == 0x40;
            invalid = (b5 & 0x80) == 0x80;

            sequenceNumber = b5 & 0x1f;

            counterReading = (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte) counterReading;
            buffer[i++] = (byte) (counterReading >> 8);
            buffer[i++] = (byte) (counterReading >> 16);
            buffer[i++] = (byte) (counterReading >> 24);

            buffer[i] = (byte) sequenceNumber;
            if (carry)
            {
                buffer[i] |= 0x20;
            }
            if (counterAdjusted)
            {
                buffer[i] |= 0x40;
            }
            if (invalid)
            {
                buffer[i] |= 0x80;
            }

            return 5;
        }

        public int GetCounterReading()
        {
            return counterReading;
        }

        public int GetSequenceNumber()
        {
            return sequenceNumber;
        }

        public bool IsCarry()
        {
            return carry;
        }

        public bool IsCounterAdjusted()
        {
            return counterAdjusted;
        }

        public bool IsInvalid()
        {
            return invalid;
        }

        public override string ToString()
        {
            return "Binary counter reading: " + counterReading + ", seq num: " + sequenceNumber + ", carry: " + carry
                   + ", counter adjusted: " + counterAdjusted + ", invalid: " + invalid;
        }
    }
}