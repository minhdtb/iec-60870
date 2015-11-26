using System;
using System.IO;

namespace IEC60870.IE.Base
{
    public abstract class IeAbstractQualifierOfCommand : InformationElement
    {
        protected int Value;

        protected IeAbstractQualifierOfCommand(int qualifier, bool select)
        {
            if (qualifier < 0 || qualifier > 31)
            {
                throw new ArgumentException("Qualifier is out of bound: " + qualifier);
            }

            Value = qualifier << 2;

            if (select)
            {
                Value |= 0x80;
            }
        }

        protected IeAbstractQualifierOfCommand(BinaryReader reader)
        {
            Value = reader.ReadByte();
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) Value;
            return 1;
        }

        public bool IsSelect()
        {
            return (Value & 0x80) == 0x80;
        }

        public int GetQualifier()
        {
            return (Value >> 2) & 0x1f;
        }

        public override string ToString()
        {
            return "selected: " + IsSelect() + ", qualifier: " + GetQualifier();
        }
    }
}