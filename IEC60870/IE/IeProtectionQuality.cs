using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeProtectionQuality : IeAbstractQuality
    {
        public IeProtectionQuality(bool elapsedTimeInvalid, bool blocked, bool substituted, bool notTopical,
            bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (elapsedTimeInvalid)
            {
                value |= 0x08;
            }
        }

        public IeProtectionQuality(BinaryReader reader) : base(reader)
        {
        }

        public bool isElapsedTimeInvalid()
        {
            return (value & 0x08) == 0x08;
        }

        public override string ToString()
        {
            return "Protection Quality, elapsed time invalid: " + isElapsedTimeInvalid() + ", " + base.ToString();
        }
    }
}
