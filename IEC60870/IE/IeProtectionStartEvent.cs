using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeProtectionStartEvent : InformationElement
    {
        private readonly int value;

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

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) value;
            return 1;
        }

        public bool IsGeneralStart()
        {
            return (value & 0x01) == 0x01;
        }

        public bool IsStartOperationL1()
        {
            return (value & 0x02) == 0x02;
        }

        public bool IsStartOperationL2()
        {
            return (value & 0x04) == 0x04;
        }

        public bool IsStartOperationL3()
        {
            return (value & 0x08) == 0x08;
        }

        public bool IsStartOperationIe()
        {
            return (value & 0x10) == 0x10;
        }

        public bool IsStartReverseOperation()
        {
            return (value & 0x20) == 0x20;
        }

        public override string ToString()
        {
            return "Protection start event, general start of operation: " + IsGeneralStart() +
                   ", start of operation L1: "
                   + IsStartOperationL1() + ", start of operation L2: " + IsStartOperationL2()
                   + ", start of operation L3: " + IsStartOperationL3() + ", start of operation IE(earth current): "
                   + IsStartOperationIe() + ", start of operation in reverse direction: " + IsStartReverseOperation();
        }
    }
}