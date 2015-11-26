using System;
using System.IO;
using System.Text;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeStatusAndStatusChanges : InformationElement
    {
        private readonly int value;

        public IeStatusAndStatusChanges(int value)
        {
            this.value = value;
        }

        public IeStatusAndStatusChanges(BinaryReader reader)
        {
            value = reader.ReadInt32();
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i++] = (byte) (value >> 24);
            buffer[i++] = (byte) (value >> 16);
            buffer[i++] = (byte) (value >> 8);
            buffer[i] = (byte) value;
            return 4;
        }

        public int GetValue()
        {
            return value;
        }

        public bool GetStatus(int position)
        {
            if (position < 1 || position > 16)
            {
                throw new ArgumentException("Position out of bound. Should be between 1 and 16.");
            }

            return ((value >> (position - 17)) & 0x01) == 0x01;
        }

        public bool HasStatusChanged(int position)
        {
            if (position < 1 || position > 16)
            {
                throw new ArgumentException("Position out of bound. Should be between 1 and 16.");
            }
            return ((value >> (position - 1)) & 0x01) == 0x01;
        }

        public override string ToString()
        {
            var sb1 = new StringBuilder();
            sb1.Append(((uint) value >> 16).ToString("X"));
            while (sb1.Length < 4)
            {
                sb1.Insert(0, '0'); // pad with leading zero if needed
            }

            var sb2 = new StringBuilder();
            sb2.Append((value & 0xffff).ToString("X"));
            while (sb2.Length < 4)
            {
                sb2.Insert(0, '0'); // pad with leading zero if needed
            }

            return "Status and status changes (first bit = LSB), states: " + sb1 + ", state changes: "
                   + sb2;
        }
    }
}