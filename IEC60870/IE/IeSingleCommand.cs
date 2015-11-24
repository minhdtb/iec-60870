using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSingleCommand : IeAbstractQualifierOfCommand
    {
        public IeSingleCommand(Boolean commandStateOn, int qualifier, Boolean select) : base(qualifier, select)
        {
            if (commandStateOn)
            {
                value |= 0x01;
            }
        }

        public IeSingleCommand(BinaryReader reader) : base(reader)
        {
        }

        public Boolean isCommandStateOn()
        {
            return (value & 0x01) == 0x01;
        }

        public override String ToString()
        {
            return "Single Command state on: " + isCommandStateOn() + ", " + base.ToString();
        }
    }
}
