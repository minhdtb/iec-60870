using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeValueWithTransientState : InformationElement
    {
        private readonly bool transientState;
        private readonly long value;

        public IeValueWithTransientState(int value, bool transientState)
        {
            if (value < -64 || value > 63)
            {
                throw new ArgumentException("Value has to be in the range -64..63");
            }

            this.value = value;
            this.transientState = transientState;
        }

        public IeValueWithTransientState(BinaryReader reader)
        {
            int b1 = reader.ReadByte();

            transientState = (b1 & 0x80) == 0x80;

            if ((b1 & 0x40) == 0x40)
            {
                value = b1 | 0xffffff80;
            }
            else
            {
                value = b1 & 0x3f;
            }
        }

        public override int Encode(byte[] buffer, int i)
        {
            if (transientState)
            {
                buffer[i] = (byte) (value | 0x80);
            }
            else
            {
                buffer[i] = (byte) (value & 0x7f);
            }

            return 1;
        }

        public long GetValue()
        {
            return value;
        }

        public bool IsTransientState()
        {
            return transientState;
        }

        public override string ToString()
        {
            return "Value with transient state, value: " + GetValue() + ", transient state: " + IsTransientState();
        }
    }
}