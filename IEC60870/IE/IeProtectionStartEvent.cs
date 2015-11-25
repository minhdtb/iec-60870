using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeProtectionStartEvent : InformationElement
    {
        private int value;

        public IeProtectionStartEvent(bool generalStart, bool startOperationL1, bool startOperationL2,
                bool startOperationL3, bool startOperationIe, bool startReverseOperation)
        {
            value = 0;

            if (generalStart)
            {
                value |= 0x01;
            }
            if (startOperationL1)
            {
                value |= 0x02;
            }
            if (startOperationL2)
            {
                value |= 0x04;
            }
            if (startOperationL3)
            {
                value |= 0x08;
            }
            if (startOperationIe)
            {
                value |= 0x10;
            }
            if (startReverseOperation)
            {
                value |= 0x20;
            }
        }

        public IeProtectionStartEvent(BinaryReader reader)
        {
            value = reader.ReadByte();
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;
            return 1;
        }

        public bool isGeneralStart()
        {
            return (value & 0x01) == 0x01;
        }

        public bool isStartOperationL1()
        {
            return (value & 0x02) == 0x02;
        }

        public bool isStartOperationL2()
        {
            return (value & 0x04) == 0x04;
        }

        public bool isStartOperationL3()
        {
            return (value & 0x08) == 0x08;
        }

        public bool isStartOperationIe()
        {
            return (value & 0x10) == 0x10;
        }

        public bool isStartReverseOperation()
        {
            return (value & 0x20) == 0x20;
        }

        public override string ToString()
        {
            return "Protection start event, general start of operation: " + isGeneralStart() + ", start of operation L1: "
                    + isStartOperationL1() + ", start of operation L2: " + isStartOperationL2()
                    + ", start of operation L3: " + isStartOperationL3() + ", start of operation IE(earth current): "
                    + isStartOperationIe() + ", start of operation in reverse direction: " + isStartReverseOperation();
        }
    }
}
