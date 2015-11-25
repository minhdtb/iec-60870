using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeDoublePointWithQuality : IeAbstractQuality
    {
        public enum DoublePointInformation
        {
            INDETERMINATE_OR_INTERMEDIATE,
            OFF,
            ON,
            INDETERMINATE
        }

        public IeDoublePointWithQuality(DoublePointInformation dpi, bool blocked, bool substituted,
                bool notTopical, bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            switch (dpi)
            {
                case DoublePointInformation.INDETERMINATE_OR_INTERMEDIATE:
                    break;
                case DoublePointInformation.OFF:
                    value |= 0x01;
                    break;
                case DoublePointInformation.ON:
                    value |= 0x02;
                    break;
                case DoublePointInformation.INDETERMINATE:
                    value |= 0x03;
                    break;
            }
        }

        public IeDoublePointWithQuality(BinaryReader reader) : base(reader)
        {
        }

        public DoublePointInformation getDoublePointInformation()
        {
            switch (value & 0x03)
            {
                case 0:
                    return DoublePointInformation.INDETERMINATE_OR_INTERMEDIATE;
                case 1:
                    return DoublePointInformation.OFF;
                case 2:
                    return DoublePointInformation.ON;
                default:
                    return DoublePointInformation.INDETERMINATE;
            }
        }

        public override string ToString()
        {
            return "Double point: " + getDoublePointInformation() + ", " + base.ToString();
        }
    }
}
