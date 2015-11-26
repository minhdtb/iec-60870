using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeTime24 : InformationElement
    {
        private readonly byte[] value = new byte[3];

        public IeTime24(long timestamp)
        {
            var datetime = new DateTime(timestamp);
            var ms = datetime.Millisecond + 1000*datetime.Second;

            value[0] = (byte) ms;
            value[1] = (byte) (ms >> 8);
            value[2] = (byte) datetime.Minute;
        }

        public IeTime24(int timeInMs)
        {
            var ms = timeInMs%60000;
            value[0] = (byte) ms;
            value[1] = (byte) (ms >> 8);
            value[2] = (byte) (ms >> 8);
        }

        public IeTime24(BinaryReader reader)
        {
            value = reader.ReadBytes(3);
        }

        public override int Encode(byte[] buffer, int i)
        {
            Array.Copy(value, 0, buffer, i, 3);
            return 3;
        }

        public int GetTimeInMs()
        {
            return (value[0] & 0xff) + ((value[1] & 0xff) << 8) + value[2]*6000;
        }

        public override string ToString()
        {
            return "Time24, time in ms: " + GetTimeInMs();
        }
    }
}