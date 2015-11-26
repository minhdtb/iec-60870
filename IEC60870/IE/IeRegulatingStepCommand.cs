using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public enum StepCommandState
    {
        NotPermittedA = 0,
        NextStepLower,
        NextStepHigher,
        NotPermittedB
    }

    public class IeRegulatingStepCommand : IeAbstractQualifierOfCommand
    {
        public IeRegulatingStepCommand(StepCommandState commandState, int qualifier, bool select)
            : base(qualifier, select)
        {
            Value |= (int) commandState;
        }

        public IeRegulatingStepCommand(BinaryReader reader) : base(reader)
        {
        }

        public static StepCommandState CreateStepCommandState(int code)
        {
            switch (code)
            {
                case 0:
                    return StepCommandState.NotPermittedA;
                case 1:
                    return StepCommandState.NextStepLower;
                case 2:
                    return StepCommandState.NextStepHigher;
                case 3:
                    return StepCommandState.NotPermittedB;
                default:
                    throw new ArgumentException("Invalid code");
            }
        }

        public StepCommandState GetCommandState()
        {
            return CreateStepCommandState(Value & 0x03);
        }

        public override string ToString()
        {
            return "Regulating step command state: " + GetCommandState() + ", " + base.ToString();
        }
    }
}