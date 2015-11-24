using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public enum StepCommandState
    {
        NOT_PERMITTED_A = 0,
        NEXT_STEP_LOWER,
        NEXT_STEP_HIGHER,
        NOT_PERMITTED_B
    }

    public class IeRegulatingStepCommand : IeAbstractQualifierOfCommand
    {
        public static StepCommandState createStepCommandState(int code)
        {
            switch (code)
            {
                case 0:
                    return StepCommandState.NOT_PERMITTED_A;
                case 1:
                    return StepCommandState.NEXT_STEP_LOWER;
                case 2:
                    return StepCommandState.NEXT_STEP_HIGHER;
                case 3:
                    return StepCommandState.NOT_PERMITTED_B;
                default:
                    throw new ArgumentException("Invalid code");
            }
        }

        public IeRegulatingStepCommand(StepCommandState commandState, int qualifier, Boolean select) : base(qualifier, select)
        {
            value |= (int)commandState;
        }

        public IeRegulatingStepCommand(BinaryReader reader) : base(reader)
        {
        }

        public StepCommandState getCommandState()
        {
            return createStepCommandState(value & 0x03);
        }

        public override String ToString()
        {
            return "Regulating step command state: " + getCommandState() + ", " + base.ToString();
        }
    }
}
