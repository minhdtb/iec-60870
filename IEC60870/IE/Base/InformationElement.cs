namespace IEC60870.IE.Base
{
    public abstract class InformationElement
    {
        public abstract int Encode(byte[] buffer, int i);
    }
}