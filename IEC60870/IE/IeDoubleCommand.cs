using IEC60870.IE.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC60870.IE
{
    public enum DoubleCommandState
    {
        NOT_PERMITTED_A = 0,
        OFF,
        ON,
        NOT_PERMITTED_B
    }

    public class IeDoubleCommand : IeAbstractQualifierOfCommand
    {
        
    public IeDoubleCommand(DoubleCommandState commandState, int qualifier, Boolean select) : base (qualifier, select)
    {       
        value |= commandState.getCode();
    }

    IeDoubleCommand(BinaryReader reader) : base (reader)
    {    
    }

    public DoubleCommandState getCommandState()
    {
        return DoubleCommandState.createDoubleCommandState(value & 0x03);
    }

    @Override
    public String toString()
    {
        return "Double Command state: " + getCommandState() + ", " + super.toString();
    }
}
}
