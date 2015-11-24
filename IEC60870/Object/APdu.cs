using IEC60870.Connection;
using System.IO;

namespace IEC60870.Object
{
    public class APdu
    {
        public enum APCI_TYPE
        {
            I_FORMAT, S_FORMAT, TESTFR_CON, TESTFR_ACT, STOPDT_CON, STOPDT_ACT, STARTDT_CON, STARTDT_ACT
        }

        private int sendSeqNum = 0;

        private int receiveSeqNum = 0;

        private APCI_TYPE apciType;

        private ASdu aSdu = null;

        public APdu(int sendSeqNum, int receiveSeqNum, APCI_TYPE apciType, ASdu aSdu)
        {
            this.sendSeqNum = sendSeqNum;
            this.receiveSeqNum = receiveSeqNum;
            this.apciType = apciType;
            this.aSdu = aSdu;
        }

        public APdu(BinaryReader reader, ConnectionSettings settings)
        {
            int length = reader.ReadByte() & 0xff;

            if (length < 4 || length > 253)
            {
                throw new IOException("APDU contain invalid length: " + length);
            }

            byte[] aPduHeader = reader.ReadBytes(4);

            if ((aPduHeader[0] & 0x01) == 0)
            {
                apciType = APCI_TYPE.I_FORMAT;
                sendSeqNum = ((aPduHeader[0] & 0xfe) >> 1) + ((aPduHeader[1] & 0xff) << 7);
                receiveSeqNum = ((aPduHeader[2] & 0xfe) >> 1) + ((aPduHeader[3] & 0xff) << 7);

                aSdu = new ASdu(reader, settings, length - 4);
            }
            else if ((aPduHeader[0] & 0x02) == 0)
            {
                apciType = APCI_TYPE.S_FORMAT;
                receiveSeqNum = ((aPduHeader[2] & 0xfe) >> 1) + ((aPduHeader[3] & 0xff) << 7);
            }
            else
            {
                if (aPduHeader[0] == (byte)0x83)
                {
                    apciType = APCI_TYPE.TESTFR_CON;
                }
                else if (aPduHeader[0] == 0x43)
                {
                    apciType = APCI_TYPE.TESTFR_ACT;
                }
                else if (aPduHeader[0] == 0x23)
                {
                    apciType = APCI_TYPE.STOPDT_CON;
                }
                else if (aPduHeader[0] == 0x13)
                {
                    apciType = APCI_TYPE.STOPDT_ACT;
                }
                else if (aPduHeader[0] == 0x0B)
                {
                    apciType = APCI_TYPE.STARTDT_CON;
                }
                else
                {
                    apciType = APCI_TYPE.STARTDT_ACT;
                }
            }
        }

        public int encode(byte[] buffer, ConnectionSettings settings)
        {
            buffer[0] = 0x68;

            int length = 4;

            if (apciType == APCI_TYPE.I_FORMAT)
            {
                buffer[2] = (byte)(sendSeqNum << 1);
                buffer[3] = (byte)(sendSeqNum >> 7);
                buffer[4] = (byte)(receiveSeqNum << 1);
                buffer[5] = (byte)(receiveSeqNum >> 7);
                length += aSdu.encode(buffer, 6, settings);
            }
            else if (apciType == APCI_TYPE.STARTDT_ACT)
            {
                buffer[2] = 0x07;
                buffer[3] = 0x00;
                buffer[4] = 0x00;
                buffer[5] = 0x00;
            }
            else if (apciType == APCI_TYPE.STARTDT_CON)
            {
                buffer[2] = 0x0b;
                buffer[3] = 0x00;
                buffer[4] = 0x00;
                buffer[5] = 0x00;
            }
            else if (apciType == APCI_TYPE.S_FORMAT)
            {
                buffer[2] = 0x01;
                buffer[3] = 0x00;
                buffer[4] = (byte)(receiveSeqNum << 1);
                buffer[5] = (byte)(receiveSeqNum >> 7);
            }

            buffer[1] = (byte)length;

            return length + 2;

        }

        public APCI_TYPE getApciType()
        {
            return apciType;
        }

        public int getSendSeqNumber()
        {
            return sendSeqNum;
        }

        public int getReceiveSeqNumber()
        {
            return receiveSeqNum;
        }

        public ASdu getASdu()
        {
            return aSdu;
        }
    }
}
