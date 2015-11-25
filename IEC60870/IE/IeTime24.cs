using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeTime24 : InformationElement
    {
        private byte[] value = new byte[3];

        public IeTime24(long timestamp)
        {
            var datetime = new DateTime(timestamp);
            int ms = datetime.Millisecond + 1000 * datetime.Second;

            value[0] = (byte)ms;
            value[1] = (byte)(ms >> 8);
            value[2] = (byte)datetime.Minute;
        }

        public IeTime24(int timeInMs)
        {
            int ms = timeInMs % 60000;
            value[0] = (byte)ms;
            value[1] = (byte)(ms >> 8);
            value[2] = (byte)(ms >> 8);
        }

        public IeTime24(BinaryReader reader)
        {
            value = reader.ReadBytes(3);
        }

        public override int encode(byte[] buffer, int i)
        {
            Array.Copy(value, 0, buffer, i, 3);
            return 3;
        }

        public int getTimeInMs()
        {
            return (value[0] & 0xff) + ((value[1] & 0xff) << 8) + value[2] * 6000;
        }

        public override string ToString()
        {
            return "Time24, time in ms: " + getTimeInMs();
        }
    }
}
