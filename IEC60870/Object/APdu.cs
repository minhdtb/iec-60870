using System.IO;
using IEC60870.Connection;

namespace IEC60870.Object
{
    public class APdu
    {
        public enum ApciType
        {
            FORMAT,
            S_FORMAT,
            TESTFR_CON,
            TESTFR_ACT,
            STOPDT_CON,
            STOPDT_ACT,
            STARTDT_CON,
            STARTDT_ACT
        }

        private readonly ApciType apciType;

        private readonly ASdu aSdu;

        private readonly int receiveSeqNum;

        private readonly int sendSeqNum;

        public APdu(int sendSeqNum, int receiveSeqNum, ApciType apciType, ASdu aSdu)
        {
            this.sendSeqNum = sendSeqNum;
            this.receiveSeqNum = receiveSeqNum;
            this.apciType = apciType;
            this.aSdu = aSdu;
        }

        public APdu(BinaryReader reader, ConnectionSettings settings)
        {
            var length = reader.ReadByte() & 0xff;

            if (length < 4 || length > 253)
            {
                throw new IOException("APDU contain invalid length: " + length);
            }

            var aPduHeader = reader.ReadBytes(4);

            if ((aPduHeader[0] & 0x01) == 0)
            {
                apciType = ApciType.FORMAT;
                sendSeqNum = ((aPduHeader[0] & 0xfe) >> 1) + ((aPduHeader[1] & 0xff) << 7);
                receiveSeqNum = ((aPduHeader[2] & 0xfe) >> 1) + ((aPduHeader[3] & 0xff) << 7);

                aSdu = new ASdu(reader, settings, length - 4);
            }
            else if ((aPduHeader[0] & 0x02) == 0)
            {
                apciType = ApciType.S_FORMAT;
                receiveSeqNum = ((aPduHeader[2] & 0xfe) >> 1) + ((aPduHeader[3] & 0xff) << 7);
            }
            else
            {
                if (aPduHeader[0] == 0x83)
                {
                    apciType = ApciType.TESTFR_CON;
                }
                else if (aPduHeader[0] == 0x43)
                {
                    apciType = ApciType.TESTFR_ACT;
                }
                else if (aPduHeader[0] == 0x23)
                {
                    apciType = ApciType.STOPDT_CON;
                }
                else if (aPduHeader[0] == 0x13)
                {
                    apciType = ApciType.STOPDT_ACT;
                }
                else if (aPduHeader[0] == 0x0B)
                {
                    apciType = ApciType.STARTDT_CON;
                }
                else
                {
                    apciType = ApciType.STARTDT_ACT;
                }
            }
        }

        public int Encode(byte[] buffer, ConnectionSettings settings)
        {
            buffer[0] = 0x68;

            var length = 4;

            if (apciType == ApciType.FORMAT)
            {
                buffer[2] = (byte) (sendSeqNum << 1);
                buffer[3] = (byte) (sendSeqNum >> 7);
                buffer[4] = (byte) (receiveSeqNum << 1);
                buffer[5] = (byte) (receiveSeqNum >> 7);
                length += aSdu.Encode(buffer, 6, settings);
            }
            else if (apciType == ApciType.STARTDT_ACT)
            {
                buffer[2] = 0x07;
                buffer[3] = 0x00;
                buffer[4] = 0x00;
                buffer[5] = 0x00;
            }
            else if (apciType == ApciType.STARTDT_CON)
            {
                buffer[2] = 0x0b;
                buffer[3] = 0x00;
                buffer[4] = 0x00;
                buffer[5] = 0x00;
            }
            else if (apciType == ApciType.S_FORMAT)
            {
                buffer[2] = 0x01;
                buffer[3] = 0x00;
                buffer[4] = (byte) (receiveSeqNum << 1);
                buffer[5] = (byte) (receiveSeqNum >> 7);
            }

            buffer[1] = (byte) length;

            return length + 2;
        }

        public ApciType GetApciType()
        {
            return apciType;
        }

        public int GetSendSeqNumber()
        {
            return sendSeqNum;
        }

        public int GetReceiveSeqNumber()
        {
            return receiveSeqNum;
        }

        public ASdu GetASdu()
        {
            return aSdu;
        }
    }
}