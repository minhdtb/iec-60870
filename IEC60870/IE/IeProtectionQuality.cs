using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeProtectionQuality : IeAbstractQuality
    {
        public IeProtectionQuality(Boolean elapsedTimeInvalid, Boolean blocked, Boolean substituted, Boolean notTopical,
            Boolean invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (elapsedTimeInvalid)
            {
                value |= 0x08;
            }
        }

        IeProtectionQuality(BinaryReader reader) : base(reader)
        {
        }

        public Boolean isElapsedTimeInvalid()
        {
            return (value & 0x08) == 0x08;
        }

        public override String ToString()
        {
            return "Protection Quality, elapsed time invalid: " + isElapsedTimeInvalid() + ", " + base.toString();
        }
    }
}
