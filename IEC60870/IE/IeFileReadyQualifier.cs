using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeFileReadyQualifier : InformationElement
    {
        private int value;
        private bool negativeConfirm;

        public IeFileReadyQualifier(int value, bool negativeConfirm)
        {
            this.value = value;
            this.negativeConfirm = negativeConfirm;
        }

        public IeFileReadyQualifier(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            value = b1 & 0x7f;
            negativeConfirm = ((b1 & 0x80) == 0x80);
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;
            if (negativeConfirm)
            {
                buffer[i] |= 0x80;
            }
            return 1;
        }

        public int getValue()
        {
            return value;
        }

        public bool isNegativeConfirm()
        {
            return negativeConfirm;
        }

        public override string ToString()
        {
            return "File ready qualifier: " + value + ", negative confirm: " + negativeConfirm;
        }
    }
}
