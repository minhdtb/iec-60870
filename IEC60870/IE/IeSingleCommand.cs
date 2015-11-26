using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeSingleCommand : IeAbstractQualifierOfCommand
    {
        public IeSingleCommand(bool commandStateOn, int qualifier, bool select) : base(qualifier, select)
        {
            if (commandStateOn)
            {
                Value |= 0x01;
            }
        }

        public IeSingleCommand(BinaryReader reader) : base(reader)
        {
        }

        public bool IsCommandStateOn()
        {
            return (Value & 0x01) == 0x01;
        }

        public override string ToString()
        {
            return "Single Command state on: " + IsCommandStateOn() + ", " + base.ToString();
        }
    }
}