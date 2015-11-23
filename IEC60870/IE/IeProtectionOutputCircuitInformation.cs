using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    class IeProtectionOutputCircuitInformation : InformationElement
    {
        private int value;

        public IeProtectionOutputCircuitInformation(Boolean generalCommand, Boolean commandToL1, Boolean commandToL2, Boolean commandToL3)
        {
            value = 0;

            if (generalCommand)
            {
                value |= 0x01;
            }
            if (commandToL1)
            {
                value |= 0x02;
            }
            if (commandToL2)
            {
                value |= 0x04;
            }
            if (commandToL3)
            {
                value |= 0x08;
            }
        }

        IeProtectionOutputCircuitInformation(BinaryReader reader)
        {
            value = reader.ReadByte();
        }


        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;
            return 1;
        }

        public Boolean isGeneralCommand()
        {
            return (value & 0x01) == 0x01;
        }

        public Boolean isCommandToL1()
        {
            return (value & 0x02) == 0x02;
        }

        public Boolean isCommandToL2()
        {
            return (value & 0x04) == 0x04;
        }

        public Boolean isCommandToL3()
        {
            return (value & 0x08) == 0x08;
        }

        public override String ToString()
        {
            return "Protection output circuit information, general command: " + isGeneralCommand() + ", command to L1: "
                    + isCommandToL1() + ", command to L2: " + isCommandToL2() + ", command to L3: " + isCommandToL3();
        }
    }
}
