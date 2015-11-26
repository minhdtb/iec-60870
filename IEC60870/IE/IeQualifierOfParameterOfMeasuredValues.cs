using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeQualifierOfParameterOfMeasuredValues : InformationElement
    {
        private readonly bool change;
        private readonly int kindOfParameter;
        private readonly bool notInOperation;

        public IeQualifierOfParameterOfMeasuredValues(int kindOfParameter, bool change, bool notInOperation)
        {
            this.kindOfParameter = kindOfParameter;
            this.change = change;
            this.notInOperation = notInOperation;
        }

        public IeQualifierOfParameterOfMeasuredValues(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            kindOfParameter = b1 & 0x3f;
            change = (b1 & 0x40) == 0x40;
            notInOperation = (b1 & 0x80) == 0x80;
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) kindOfParameter;
            if (change)
            {
                buffer[i] |= 0x40;
            }
            if (notInOperation)
            {
                buffer[i] |= 0x80;
            }
            return 1;
        }

        public int GetKindOfParameter()
        {
            return kindOfParameter;
        }

        public bool IsChange()
        {
            return change;
        }

        public bool IsNotInOperation()
        {
            return notInOperation;
        }

        public override string ToString()
        {
            return "Qualifier of parameter of measured values, kind of parameter: " + kindOfParameter + ", change: "
                   + change + ", not in operation: " + notInOperation;
        }
    }
}