using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQuality : IeAbstractQuality
    {
        public IeQuality(Boolean overflow, Boolean blocked, Boolean substituted, Boolean notTopical, Boolean invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (overflow)
            {
                value |= 0x01;
            }
        }

        IeQuality(BinaryReader reader) : base(reader)
        {
        }

        public Boolean isOverflow() 
        {
            return (value & 0x01) == 0x01;
        }

        public override String ToString()
        {
            return "Quality, overflow: " + isOverflow() + ", " + base.ToString();
        }
    }
}
