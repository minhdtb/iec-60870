using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeCauseOfInitialization : InformationElement
    {
        private readonly bool initAfterParameterChange;
        private readonly int value;

        public IeCauseOfInitialization(int value, bool initAfterParameterChange)
        {
            if (value < 0 || value > 127)
            {
                throw new ArgumentException("Value has to be in the range 0..127");
            }

            this.value = value;
            this.initAfterParameterChange = initAfterParameterChange;
        }

        public IeCauseOfInitialization(BinaryReader reader)
        {
            int b1 = reader.ReadByte();

            initAfterParameterChange = (b1 & 0x80) == 0x80;

            value = b1 & 0x7f;
        }

        public override int Encode(byte[] buffer, int i)
        {
            if (initAfterParameterChange)
            {
                buffer[i] = (byte) (value | 0x80);
            }
            else
            {
                buffer[i] = (byte) value;
            }

            return 1;
        }

        public int GetValue()
        {
            return value;
        }

        public bool IsInitAfterParameterChange()
        {
            return initAfterParameterChange;
        }

        public override string ToString()
        {
            return "Cause of initialization: " + value + ", init after parameter change: " + initAfterParameterChange;
        }
    }
}