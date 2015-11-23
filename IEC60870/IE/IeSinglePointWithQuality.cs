using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSinglePointWithQuality : IeAbstractQuality
    {
        public IeSinglePointWithQuality(Boolean on, Boolean blocked, Boolean substituted, Boolean notTopical,
            Boolean invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (on)
            {
                value |= 0x01;
            }
        }

        public IeSinglePointWithQuality(BinaryReader reader) : base(reader)
        {
        }

        public Boolean isOn()
        {
            return (value & 0x01) == 0x01;
        }

        public override String ToString()
        {
            return "Single Point, is on: " + isOn() + ", " + base.ToString();
        }
    }
}
