using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeBinaryCounterReading : InformationElement
    {
        private int counterReading;
        private int sequenceNumber;
        private bool carry;
        private bool counterAdjusted;
        private bool invalid;

        public IeBinaryCounterReading(int counterReading, int sequenceNumber, bool carry, bool counterAdjusted, bool invalid)
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

            carry = ((b5 & 0x20) == 0x20);
            counterAdjusted = ((b5 & 0x40) == 0x40);
            invalid = ((b5 & 0x80) == 0x80);

            sequenceNumber = b5 & 0x1f;

            counterReading = (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte)counterReading;
            buffer[i++] = (byte)(counterReading >> 8);
            buffer[i++] = (byte)(counterReading >> 16);
            buffer[i++] = (byte)(counterReading >> 24);

            buffer[i] = (byte)sequenceNumber;
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

        public int getCounterReading()
        {
            return counterReading;
        }

        public int getSequenceNumber()
        {
            return sequenceNumber;
        }

        public bool isCarry()
        {
            return carry;
        }

        public bool isCounterAdjusted()
        {
            return counterAdjusted;
        }

        public bool isInvalid()
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
