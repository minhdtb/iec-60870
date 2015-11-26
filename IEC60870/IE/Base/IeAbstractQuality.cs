using System.IO;

namespace IEC60870.IE.Base
{
    public abstract class IeAbstractQuality : InformationElement
    {
        protected int Value;

        protected IeAbstractQuality(bool blocked, bool substituted, bool notTopical, bool invalid)
        {
            Value = 0;

            if (blocked)
            {
                Value |= 0x10;
            }
            if (substituted)
            {
                Value |= 0x20;
            }
            if (notTopical)
            {
                Value |= 0x40;
            }
            if (invalid)
            {
                Value |= 0x80;
            }
        }

        protected IeAbstractQuality(BinaryReader reader)
        {
            Value = reader.ReadByte();
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) Value;
            return 1;
        }

        public bool IsBlocked()
        {
            return (Value & 0x10) == 0x10;
        }

        public bool IsSubstituted()
        {
            return (Value & 0x20) == 0x20;
        }

        public bool IsNotTopical()
        {
            return (Value & 0x40) == 0x40;
        }

        public bool IsInvalid()
        {
            return (Value & 0x80) == 0x80;
        }

        public override string ToString()
        {
            return "blocked: " + IsBlocked() + ", substituted: " + IsSubstituted() + ", not topical: " + IsNotTopical()
                   + ", invalid: " + IsInvalid();
        }
    }
}