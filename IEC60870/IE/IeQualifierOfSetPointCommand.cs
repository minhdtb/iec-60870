using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeQualifierOfSetPointCommand : InformationElement
    {
        private int ql;
        private Boolean select;

        public IeQualifierOfSetPointCommand(int ql, Boolean select)
        {
            this.ql = ql;
            this.select = select;
        }

        IeQualifierOfSetPointCommand(BinaryReader reader)
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

        public Boolean isSelect()
        {
            return select;
        }

        public override String ToString()
        {
            return "Qualifier of set point command, QL: " + ql + ", select: " + select;
        }
    }
}
