using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSingleCommand : IeAbstractQualifierOfCommand
    {
        public IeSingleCommand(bool commandStateOn, int qualifier, bool select) : base(qualifier, select)
        {
            if (commandStateOn)
            {
                value |= 0x01;
            }
        }

        public IeSingleCommand(BinaryReader reader) : base(reader)
        {
        }

        public bool isCommandStateOn()
        {
            return (value & 0x01) == 0x01;
        }

        public override string ToString()
        {
            return "Single Command state on: " + isCommandStateOn() + ", " + base.ToString();
        }
    }
}
