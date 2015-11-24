using IEC60870.IE.Base;
using System;
using System.IO;
using System.Text;

namespace IEC60870.IE
{
    public class IeTime56 : InformationElement
    {
        private byte[] value = new byte[7];

        public IeTime56(long timestamp, TimeZone timeZone, Boolean invalid)
        {
        }

        public IeTime56(long timestamp) : this(timestamp, TimeZone.CurrentTimeZone, false)
        {
        }

        public IeTime56(byte[] value)
        {
            for (int i = 0; i < 7; i++)
            {
                this.value[i] = value[i];
            }
        }

        public IeTime56(BinaryReader reader)
        {
            value = reader.ReadBytes(7);
        }

        public override int encode(byte[] buffer, int i)
        {
            Array.Copy(value, 0, buffer, i, 7);
            return 7;
        }

        public long getTimestamp(int startOfCentury, TimeZone timeZone)
        {
            int century = startOfCentury / 100 * 100;
            if (value[6] < (startOfCentury % 100))
            {
                century += 100;
            }

            return -1;
        }

        public long getTimestamp(int startOfCentury)
        {
            return getTimestamp(startOfCentury, TimeZone.CurrentTimeZone);
        }

        public long getTimestamp()
        {
            return getTimestamp(1970, TimeZone.CurrentTimeZone);
        }

        public int getMillisecond()
        {
            return (((value[0] & 0xff) + ((value[1] & 0xff) << 8))) % 1000;
        }

        public int getSecond()
        {
            return (((value[0] & 0xff) + ((value[1] & 0xff) << 8))) / 1000;
        }

        public int getMinute()
        {
            return value[2] & 0x3f;
        }

        public int getHour()
        {
            return value[3] & 0x1f;
        }

        public int getDayOfWeek()
        {
            return (value[4] & 0xe0) >> 5;
        }

        public int getDayOfMonth()
        {
            return value[4] & 0x1f;
        }

        public int getMonth()
        {
            return value[5] & 0x0f;
        }

        public int getYear()
        {
            return value[6] & 0x7f;
        }

        public Boolean isSummerTime()
        {
            return (value[3] & 0x80) == 0x80;
        }

        public Boolean isInvalid()
        {
            return (value[2] & 0x80) == 0x80;
        }

        public override String ToString()
        {
            StringBuilder builder = new StringBuilder("Time56: ");
            appendWithNumDigits(builder, getDayOfMonth(), 2);
            builder.Append("-");
            appendWithNumDigits(builder, getMonth(), 2);
            builder.Append("-");
            appendWithNumDigits(builder, getYear(), 2);
            builder.Append(" ");
            appendWithNumDigits(builder, getHour(), 2);
            builder.Append(":");
            appendWithNumDigits(builder, getMinute(), 2);
            builder.Append(":");
            appendWithNumDigits(builder, getSecond(), 2);
            builder.Append(":");
            appendWithNumDigits(builder, getMillisecond(), 3);

            if (isSummerTime())
            {
                builder.Append(" DST");
            }

            if (isInvalid())
            {
                builder.Append(", invalid");
            }

            return builder.ToString();
        }

        private void appendWithNumDigits(StringBuilder builder, int value, int numDigits)
        {
            int i = numDigits - 1;
            while (i < numDigits && value < Math.Pow(10, i))
            {
                builder.Append("0");
                i--;
            }
            builder.Append(value);
        }
    }
}
