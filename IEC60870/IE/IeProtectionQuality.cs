using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeProtectionQuality : IeAbstractQuality
    {
        public IeProtectionQuality(bool elapsedTimeInvalid, bool blocked, bool substituted, bool notTopical,
            bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (elapsedTimeInvalid)
            {
                Value |= 0x08;
            }
        }

        public IeProtectionQuality(BinaryReader reader) : base(reader)
        {
        }

        public bool IsElapsedTimeInvalid()
        {
            return (Value & 0x08) == 0x08;
        }

        public override string ToString()
        {
            return "Protection Quality, elapsed time invalid: " + IsElapsedTimeInvalid() + ", " + base.ToString();
        }
    }
}