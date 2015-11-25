using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQualifierOfSetPointCommand : InformationElement
    {
        private int ql;
        private bool select;

        public IeQualifierOfSetPointCommand(int ql, bool select)
        {
            this.ql = ql;
            this.select = select;
        }

        public IeQualifierOfSetPointCommand(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            ql = b1 & 0x7f;
            select = ((b1 & 0x80) == 0x80);
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)ql;
            if (select)
            {
                buffer[i] |= 0x80;
            }
            return 1;
        }

        public int getQl()
        {
            return ql;
        }

        public bool isSelect()
        {
            return select;
        }

        public override string ToString()
        {
            return "Qualifier of set point command, QL: " + ql + ", select: " + select;
        }
    }
}
