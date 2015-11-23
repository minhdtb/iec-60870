using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeValueWithTransientState : InformationElement
    {
        private long value;
        private Boolean transientState;

        public IeValueWithTransientState(int value, Boolean transientState)
        {
            if (value < -64 || value > 63)
            {
                throw new ArgumentException("Value has to be in the range -64..63");
            }

            this.value = value;
            this.transientState = transientState;
        }

        IeValueWithTransientState(BinaryReader reader)
        {
            int b1 = reader.ReadByte();

            transientState = ((b1 & 0x80) == 0x80);

            if ((b1 & 0x40) == 0x40)
            {
                value = b1 | 0xffffff80;
            }
            else
            {
                value = b1 & 0x3f;
            }

        }

        public override int encode(byte[] buffer, int i)
        {
            if (transientState)
            {
                buffer[i] = (byte)(value | 0x80);
            }
            else
            {
                buffer[i] = (byte)(value & 0x7f);
            }

            return 1;
        }

        public long getValue()
        {
            return value;
        }

        public Boolean isTransientState()
        {
            return transientState;
        }

        public override String ToString()
        {
            return "Value with transient state, value: " + getValue() + ", transient state: " + isTransientState();
        }
    }
}
