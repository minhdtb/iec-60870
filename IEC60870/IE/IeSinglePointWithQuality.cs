using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSinglePointWithQuality : IeAbstractQuality
    {
        public IeSinglePointWithQuality(bool on, bool blocked, bool substituted, bool notTopical,
            bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (on)
            {
                value |= 0x01;
            }
        }

        public IeSinglePointWithQuality(BinaryReader reader) : base(reader)
        {
        }

        public bool isOn()
        {
            return (value & 0x01) == 0x01;
        }

        public override string ToString()
        {
            return "Single Point, is on: " + isOn() + ", " + base.ToString();
        }
    }
}
