using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeSinglePointWithQuality : IeAbstractQuality
    {
        public IeSinglePointWithQuality(bool on, bool blocked, bool substituted, bool notTopical,
            bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            if (on)
            {
                Value |= 0x01;
            }
        }

        public IeSinglePointWithQuality(BinaryReader reader) : base(reader)
        {
        }

        public bool IsOn()
        {
            return (Value & 0x01) == 0x01;
        }

        public override string ToString()
        {
            return "Single Point, is on: " + IsOn() + ", " + base.ToString();
        }
    }
}