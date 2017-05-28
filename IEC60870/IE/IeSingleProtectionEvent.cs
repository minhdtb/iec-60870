using System;
using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeSingleProtectionEvent : InformationElement
    {
        public enum EventState
        {
            Indeterminate,
            Off,
            On
        }

        private readonly int value;

        public IeSingleProtectionEvent(EventState eventState, bool elapsedTimeInvalid, bool blocked,
            bool substituted, bool notTopical, bool eventInvalid)
        {
            value = 0;

            switch (eventState)
            {
                case EventState.Off:
                    value |= 0x01;
                    break;
                case EventState.On:
                    value |= 0x02;
                    break;
                case EventState.Indeterminate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventState), eventState, null);
            }

            if (elapsedTimeInvalid)
            {
                value |= 0x08;
            }
            if (blocked)
            {
                value |= 0x10;
            }
            if (substituted)
            {
                value |= 0x20;
            }
            if (notTopical)
            {
                value |= 0x40;
            }
            if (eventInvalid)
            {
                value |= 0x80;
            }
        }

        public IeSingleProtectionEvent(BinaryReader reader)
        {
            value = reader.ReadByte();
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) value;
            return 1;
        }

        public EventState GetEventState()
        {
            switch (value & 0x03)
            {
                case 1:
                    return EventState.Off;
                case 2:
                    return EventState.On;
                default:
                    return EventState.Indeterminate;
            }
        }

        public bool IsElapsedTimeInvalid()
        {
            return (value & 0x08) == 0x08;
        }

        public bool IsBlocked()
        {
            return (value & 0x10) == 0x10;
        }

        public bool IsSubstituted()
        {
            return (value & 0x20) == 0x20;
        }

        public bool IsNotTopical()
        {
            return (value & 0x40) == 0x40;
        }

        public bool IsEventInvalid()
        {
            return (value & 0x80) == 0x80;
        }

        public override string ToString()
        {
            return "Single protection event, elapsed time invalid: " + IsElapsedTimeInvalid() + ", blocked: " +
                   IsBlocked()
                   + ", substituted: " + IsSubstituted() + ", not topical: " + IsNotTopical() + ", event invalid: "
                   + IsEventInvalid();
        }
    }
}