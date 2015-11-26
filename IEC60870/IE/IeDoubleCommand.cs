using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public enum DoubleCommandState
    {
        NotPermittedA = 0,
        Off,
        On,
        NotPermittedB
    }

    public class IeDoubleCommand : IeAbstractQualifierOfCommand
    {
        public IeDoubleCommand(DoubleCommandState commandState, int qualifier, bool select) : base(qualifier, select)
        {
            Value |= (int) commandState;
        }

        public IeDoubleCommand(BinaryReader reader) : base(reader)
        {
        }

        public static DoubleCommandState CreateDoubleCommandState(int code)
        {
            switch (code)
            {
                case 0:
                    return DoubleCommandState.NotPermittedA;
                case 1:
                    return DoubleCommandState.Off;
                case 2:
                    return DoubleCommandState.On;
                case 3:
                    return DoubleCommandState.NotPermittedB;
                default:
                    throw new ArgumentException("Invalid code");
            }
        }

        public DoubleCommandState GetCommandState()
        {
            return CreateDoubleCommandState(Value & 0x03);
        }

        public override string ToString()
        {
            return "Double command state: " + GetCommandState() + ", " + base.ToString();
        }
    }
}