namespace IEC60870.IE.Base
{
    public abstract class InformationElement
    {
        public abstract int encode(byte[] buffer, int i);
    }
}
