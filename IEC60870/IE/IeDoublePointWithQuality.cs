using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeDoublePointWithQuality : IeAbstractQuality
    {
        public enum DoublePointInformation
        {
            IndeterminateOrIntermediate,
            Off,
            On,
            Indeterminate
        }

        public IeDoublePointWithQuality(DoublePointInformation dpi, bool blocked, bool substituted,
            bool notTopical, bool invalid) : base(blocked, substituted, notTopical, invalid)
        {
            switch (dpi)
            {
                case DoublePointInformation.IndeterminateOrIntermediate:
                    break;
                case DoublePointInformation.Off:
                    Value |= 0x01;
                    break;
                case DoublePointInformation.On:
                    Value |= 0x02;
                    break;
                case DoublePointInformation.Indeterminate:
                    Value |= 0x03;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dpi), dpi, null);
            }
        }

        public IeDoublePointWithQuality(BinaryReader reader) : base(reader)
        {
        }

        public DoublePointInformation GetDoublePointInformation()
        {
            switch (Value & 0x03)
            {
                case 0:
                    return DoublePointInformation.IndeterminateOrIntermediate;
                case 1:
                    return DoublePointInformation.Off;
                case 2:
                    return DoublePointInformation.On;
                default:
                    return DoublePointInformation.Indeterminate;
            }
        }

        public override string ToString()
        {
            return "Double point: " + GetDoublePointInformation() + ", " + base.ToString();
        }
    }
}