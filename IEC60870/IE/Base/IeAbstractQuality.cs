using System;
using System.IO;

namespace IEC60870.IE.Base
{
    public abstract class IeAbstractQuality : InformationElement
    {
        protected int value;

        public IeAbstractQuality(bool blocked, bool substituted, bool notTopical, bool invalid)
        {
            value = 0;

            if (blocked)
            {
                value |= 0x10;
            }
            if (substituted)
            {
                value |= 0x20;
            }
            if (notTopical)
            {
                value |= 0x40;
            }
            if (invalid)
            {
                value |= 0x80;
            }
        }

        public IeAbstractQuality(BinaryReader reader)
        {
            value = reader.ReadByte();
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;
            return 1;
        }

        public bool isBlocked()
        {
            return (value & 0x10) == 0x10;
        }

        public bool isSubstituted()
        {
            return (value & 0x20) == 0x20;
        }

        public bool isNotTopical()
        {
            return (value & 0x40) == 0x40;
        }

        public bool isInvalid()
        {
            return (value & 0x80) == 0x80;
        }

        public override string ToString()
        {
            return "blocked: " + isBlocked() + ", substituted: " + isSubstituted() + ", not topical: " + isNotTopical()
                    + ", invalid: " + isInvalid();
        }
    }
}
