using IEC60870.Object;
using IEC60870.Util;
using System;
using System.IO;
using System.Net.Sockets;

namespace IEC60870.Connection
{
    public class Connection
    {
        private Socket socket;

        private BinaryWriter writer;
        private BinaryReader reader;

        private Boolean closed = false;

        private ConnectionSettings settings;
        private ConnectionEventListener aSduListener = null;

        private int sendSequenceNumber = 0;
        private int receiveSequenceNumber = 0;
        private int acknowledgedReceiveSequenceNumber = 0;
        private int acknowledgedSendSequenceNumber = 0;

        private int originatorAddress = 0;

        private byte[] buffer = new byte[255];

        private static byte[] TESTFR_CON_BUFFER = new byte[] { 0x68, 0x04, 0x83, 0x00, 0x00, 0x00 };
        private static byte[] TESTFR_ACT_BUFFER = new byte[] { 0x68, 0x04, 0x43, 0x00, 0x00, 0x00 };
        private static byte[] STARTDT_ACT_BUFFER = new byte[] { 0x68, 0x04, 0x07, 0x00, 0x00, 0x00 };
        private static byte[] STARTDT_CON_BUFFER = new byte[] { 0x68, 0x04, 0x0b, 0x00, 0x00, 0x00 };

        private class ConnectionReader : ThreadBase
        {
            private Connection innerConnection;
            public ConnectionReader(Connection connection)
            {
                innerConnection = connection;
            }

            public override void Run()
            {
                try
                {
                    var reader = innerConnection.reader;
                    while (true)
                    {
                        if (reader.ReadByte() != 0x68)
                        {
                            throw new IOException("Message does not start with 0x68");
                        }

                        APdu aPdu = new APdu(reader, innerConnection.settings);
                        switch (aPdu.getApciType())
                        {
                            case APdu.APCI_TYPE.I_FORMAT:
                                if (innerConnection.receiveSequenceNumber != aPdu.getSendSeqNumber())
                                {
                                    throw new IOException("Got unexpected send sequence number: " + aPdu.getSendSeqNumber()
                                            + ", expected: " + innerConnection.receiveSequenceNumber);
                                }

                                innerConnection.receiveSequenceNumber = (aPdu.getSendSeqNumber() + 1) % 32768;
                                innerConnection.handleReceiveSequenceNumber(aPdu);

                                int numUnconfirmedIPdusReceived = innerConnection.getSequenceNumberDifference(
                                    innerConnection.receiveSequenceNumber,
                                    innerConnection.acknowledgedReceiveSequenceNumber);
                                if (numUnconfirmedIPdusReceived > innerConnection.settings.maxUnconfirmedIPdusReceived)
                                {
                                    innerConnection.sendSFormatPdu();
                                }

                                break;
                            case APdu.APCI_TYPE.STARTDT_CON:
                                innerConnection.resetMaxIdleTimeTimer();
                                break;
                            case APdu.APCI_TYPE.STARTDT_ACT:
                                break;
                            case APdu.APCI_TYPE.S_FORMAT:
                                innerConnection.handleReceiveSequenceNumber(aPdu);
                                innerConnection.resetMaxIdleTimeTimer();
                                break;
                            case APdu.APCI_TYPE.TESTFR_ACT:
                                innerConnection.writer.Write(TESTFR_CON_BUFFER, 0, TESTFR_CON_BUFFER.Length);
                                innerConnection.writer.Flush();
                                innerConnection.resetMaxIdleTimeTimer();
                                break;
                            case APdu.APCI_TYPE.TESTFR_CON:
                                innerConnection.resetMaxIdleTimeTimer();
                                break;
                            default:
                                throw new IOException("Got unexpected message with APCI Type: " + aPdu.getApciType());
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new IOException(e.Message);
                }
            }
        }

        public Connection(Socket socket, ConnectionSettings settings)
        {
            this.socket = socket;
            this.settings = settings;

            NetworkStream ns = new NetworkStream(this.socket);
            writer = new BinaryWriter(ns);
            reader = new BinaryReader(ns);

            ConnectionReader connectionReader = new ConnectionReader(this);
            connectionReader.Start();
        }

        private void handleReceiveSequenceNumber(APdu aPdu)
        {
            if (acknowledgedSendSequenceNumber != aPdu.getReceiveSeqNumber())
            {
                if (getSequenceNumberDifference(aPdu.getReceiveSeqNumber(), acknowledgedSendSequenceNumber) > getNumUnconfirmedIPdusSent())
                {
                    throw new IOException("Got unexpected receive sequence number: " + aPdu.getReceiveSeqNumber()
                            + ", expected a number between: " + acknowledgedSendSequenceNumber + " and "
                            + sendSequenceNumber);
                }

                acknowledgedSendSequenceNumber = aPdu.getReceiveSeqNumber();
            }
        }

        public void close()
        {
            if (!closed)
            {
                closed = true;

                try
                {
                    writer.Close();
                }
                catch (Exception e)
                {
                }

                try
                {
                    reader.Close();
                }
                catch (Exception e)
                {
                }
            }
        }

        public void send(ASdu aSdu)
        {
            acknowledgedReceiveSequenceNumber = receiveSequenceNumber;
            APdu requestAPdu = new APdu(sendSequenceNumber, receiveSequenceNumber, APdu.APCI_TYPE.I_FORMAT, aSdu);
            sendSequenceNumber = (sendSequenceNumber + 1) % 32768;

            int length = requestAPdu.encode(buffer, settings);
            writer.Write(buffer, 0, length);
            writer.Flush();

            resetMaxIdleTimeTimer();
        }

        private void sendSFormatPdu()
        {
            APdu requestAPdu = new APdu(0, receiveSequenceNumber, APdu.APCI_TYPE.S_FORMAT, null);
            requestAPdu.encode(buffer, settings);

            writer.Write(buffer, 0, 6);
            writer.Flush();

            acknowledgedReceiveSequenceNumber = receiveSequenceNumber;

            resetMaxIdleTimeTimer();
        }

        private void resetMaxIdleTimeTimer()
        {
        }

        private int getSequenceNumberDifference(int x, int y)
        {
            int difference = x - y;
            if (difference < 0)
            {
                difference += 32768;
            }
            return difference;
        }

        public int getNumUnconfirmedIPdusSent()
        {
            return getSequenceNumberDifference(sendSequenceNumber, acknowledgedSendSequenceNumber);
        }
    }
}
