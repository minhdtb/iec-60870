using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeQuality : IeAbstractQuality
    {
        public IeQuality(bool overflow, bool blocked, bool substituted, bool notTopical, bool invalid)
            : base(blocked, substituted, notTopical, invalid)
        {
            if (overflow)
            {
                Value |= 0x01;
            }
        }

        public IeQuality(BinaryReader reader) : base(reader)
        {
        }

        public bool IsOverflow()
        {
            return (Value & 0x01) == 0x01;
        }

        public override string ToString()
        {
            return "Quality, overflow: " + IsOverflow() + ", " + base.ToString();
        }
    }
}