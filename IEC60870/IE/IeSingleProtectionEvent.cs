using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeSingleProtectionEvent : InformationElement
    {
        private int value;

        public enum EventState
        {
            INDETERMINATE, OFF, ON
        }

        public IeSingleProtectionEvent(EventState eventState, bool elapsedTimeInvalid, bool blocked,
                bool substituted, bool notTopical, bool eventInvalid)
        {
            value = 0;

            switch (eventState)
            {
                case EventState.OFF:
                    value |= 0x01;
                    break;
                case EventState.ON:
                    value |= 0x02;
                    break;
                default:
                    break;
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

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)value;
            return 1;
        }

        public EventState getEventState()
        {
            switch (value & 0x03)
            {
                case 1:
                    return EventState.OFF;
                case 2:
                    return EventState.ON;
                default:
                    return EventState.INDETERMINATE;
            }
        }

        public bool isElapsedTimeInvalid()
        {
            return (value & 0x08) == 0x08;
        }

        public bool isBlocked()
        {
            return (value & 0x10) == 0x10;
        }

        public bool isSubstituted()
        {
            return (value & 0x20) == 0x20;
        }

        public bool isNotTopical()
        {
            return (value & 0x40) == 0x40;
        }

        public bool isEventInvalid()
        {
            return (value & 0x80) == 0x80;
        }

        public override string ToString()
        {
            return "Single protection event, elapsed time invalid: " + isElapsedTimeInvalid() + ", blocked: " + isBlocked()
                    + ", substituted: " + isSubstituted() + ", not topical: " + isNotTopical() + ", event invalid: "
                    + isEventInvalid();
        }

    }
}
