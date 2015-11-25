using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQuality : IeAbstractQuality
    {
        public IeQuality(bool overflow, bool blocked, bool substituted, bool notTopical, bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (overflow)
            {
                value |= 0x01;
            }
        }

        public IeQuality(BinaryReader reader) : base(reader)
        {
        }

        public bool isOverflow() 
        {
            return (value & 0x01) == 0x01;
        }

        public override string ToString()
        {
            return "Quality, overflow: " + isOverflow() + ", " + base.ToString();
        }
    }
}
