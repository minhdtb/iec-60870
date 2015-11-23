using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeCauseOfInitialization : InformationElement
    {
        private int value;
        private Boolean initAfterParameterChange;

        public IeCauseOfInitialization(int value, Boolean initAfterParameterChange)
        {
            if (value < 0 || value > 127)
            {
                throw new ArgumentException("Value has to be in the range 0..127");
            }

            this.value = value;
            this.initAfterParameterChange = initAfterParameterChange;
        }

        IeCauseOfInitialization(BinaryReader reader)
        {
            int b1 = reader.ReadByte();

            initAfterParameterChange = ((b1 & 0x80) == 0x80);

            value = b1 & 0x7f;
        }

        public override int encode(byte[] buffer, int i)
        {
            if (initAfterParameterChange)
            {
                buffer[i] = (byte)(value | 0x80);
            }
            else
            {
                buffer[i] = (byte)value;
            }

            return 1;
        }

        public int getValue()
        {
            return value;
        }

        public Boolean isInitAfterParameterChange()
        {
            return initAfterParameterChange;
        }

        public override String ToString()
        {
            return "Cause of initialization: " + value + ", init after parameter change: " + initAfterParameterChange;
        }
    }
}
