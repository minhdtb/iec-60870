using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
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

        private bool closed = false;

        private ConnectionSettings settings;

        public ConnectionEventListener.newASdu newASdu = null;
        public ConnectionEventListener.connectionClosed connectionClosed = null;

        private int sendSequenceNumber = 0;
        private int receiveSequenceNumber = 0;
        private int acknowledgedReceiveSequenceNumber = 0;
        private int acknowledgedSendSequenceNumber = 0;

        private int originatorAddress = 0;
        private IOException closedIOException = null;

        private byte[] buffer = new byte[255];

        private static byte[] TESTFR_CON_BUFFER = new byte[] { 0x68, 0x04, 0x83, 0x00, 0x00, 0x00 };
        private static byte[] TESTFR_ACT_BUFFER = new byte[] { 0x68, 0x04, 0x43, 0x00, 0x00, 0x00 };
        private static byte[] STARTDT_ACT_BUFFER = new byte[] { 0x68, 0x04, 0x07, 0x00, 0x00, 0x00 };
        private static byte[] STARTDT_CON_BUFFER = new byte[] { 0x68, 0x04, 0x0b, 0x00, 0x00, 0x00 };

        private RunTask maxTimeNoAckSentFuture = null;

        private RunTask maxTimeNoAckReceivedFuture = null;

        private RunTask maxIdleTimeTimerFuture = null;

        private RunTask maxTimeNoTestConReceivedFuture = null;

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

                                if (innerConnection.newASdu != null)
                                {
                                    innerConnection.newASdu(aPdu.getASdu());
                                }

                                int numUnconfirmedIPdusReceived = innerConnection.getSequenceNumberDifference(
                                    innerConnection.receiveSequenceNumber,
                                    innerConnection.acknowledgedReceiveSequenceNumber);
                                if (numUnconfirmedIPdusReceived > innerConnection.settings.maxUnconfirmedIPdusReceived)
                                {
                                    innerConnection.sendSFormatPdu();
                                    if (innerConnection.maxTimeNoAckSentFuture != null)
                                    {
                                        innerConnection.maxTimeNoAckSentFuture.Cancel();
                                        innerConnection.maxTimeNoAckSentFuture = null;
                                    }
                                }
                                else
                                {
                                    if (innerConnection.maxTimeNoAckSentFuture == null)
                                    {
                                        innerConnection.maxTimeNoAckSentFuture = PeriodicTaskFactory.Start(() =>
                                        {
                                            innerConnection.sendSFormatPdu();
                                        },
                                        innerConnection.settings.maxTimeNoAckSent);
                                    };
                                }

                                innerConnection.resetMaxIdleTimeTimer();
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
                                if (innerConnection.maxTimeNoTestConReceivedFuture != null)
                                {
                                    innerConnection.maxTimeNoTestConReceivedFuture.Cancel();
                                    innerConnection.maxTimeNoTestConReceivedFuture = null;
                                }
                                innerConnection.resetMaxIdleTimeTimer();
                                break;
                            default:
                                throw new IOException("Got unexpected message with APCI Type: " + aPdu.getApciType());
                        }
                    }
                }
                catch (IOException e)
                {
                    innerConnection.closedIOException = e;
                }
                catch (Exception e)
                {
                    innerConnection.closedIOException = new IOException(e.Message);
                }
                finally
                {
                    if (innerConnection.connectionClosed != null)
                    {
                        innerConnection.connectionClosed(innerConnection.closedIOException);
                    }

                    innerConnection.close();
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
                    throw new IOException(e.Message);
                }

                try
                {
                    reader.Close();
                }
                catch (Exception e)
                {
                    throw new IOException(e.Message);
                }
            }
        }

        public void startDataTransfer()
        {
            writer.Write(STARTDT_ACT_BUFFER, 0, STARTDT_ACT_BUFFER.Length);
            writer.Flush();
        }

        public void send(ASdu aSdu)
        {
            acknowledgedReceiveSequenceNumber = receiveSequenceNumber;
            APdu requestAPdu = new APdu(sendSequenceNumber, receiveSequenceNumber, APdu.APCI_TYPE.I_FORMAT, aSdu);
            sendSequenceNumber = (sendSequenceNumber + 1) % 32768;

            if (maxTimeNoAckSentFuture != null)
            {
                maxTimeNoAckSentFuture.Cancel();
                maxTimeNoAckSentFuture = null;
            }

            if (maxTimeNoAckReceivedFuture == null)
            {
                scheduleMaxTimeNoAckReceivedFuture();
            }

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

        public void sendConfirmation(ASdu aSdu)
        {
            CauseOfTransmission cot = aSdu.getCauseOfTransmission();

            if (cot == CauseOfTransmission.ACTIVATION)
            {
                cot = CauseOfTransmission.ACTIVATION_CON;
            }
            else if (cot == CauseOfTransmission.DEACTIVATION)
            {
                cot = CauseOfTransmission.DEACTIVATION_CON;
            }

            send(new ASdu(aSdu.getTypeIdentification(), aSdu.isSequenceOfElements, cot, aSdu.isTestFrame(),
                aSdu.isNegativeConfirm(), aSdu.getOriginatorAddress(), aSdu.getCommonAddress(),
                aSdu.getInformationObjects()));
        }

        public void singleCommand(int commonAddress, int informationObjectAddress, IeSingleCommand singleCommand)
        {
            CauseOfTransmission cot;

            if (singleCommand.isCommandStateOn())
            {
                cot = CauseOfTransmission.ACTIVATION;
            }
            else
            {
                cot = CauseOfTransmission.DEACTIVATION;
            }

            ASdu aSdu = new ASdu(TypeId.C_SC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { singleCommand } }) });

            send(aSdu);
        }

        public void singleCommandWithTimeTag(int commonAddress, int informationObjectAddress,
            IeSingleCommand singleCommand, IeTime56 timeTag)
        {
            CauseOfTransmission cot;

            if (singleCommand.isCommandStateOn())
            {
                cot = CauseOfTransmission.ACTIVATION;
            }
            else
            {
                cot = CauseOfTransmission.DEACTIVATION;
            }

            ASdu aSdu = new ASdu(TypeId.C_SC_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { singleCommand, timeTag } }) });

            send(aSdu);
        }

        public void doubleCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeDoubleCommand doubleCommand)
        {
            ASdu aSdu = new ASdu(TypeId.C_DC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { doubleCommand } }) });

            send(aSdu);
        }

        public void doubleCommandWithTimeTag(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeDoubleCommand doubleCommand, IeTime56 timeTag)
        {
            ASdu aSdu = new ASdu(TypeId.C_DC_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { doubleCommand, timeTag } }) });

            send(aSdu);
        }

        public void regulatingStepCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeRegulatingStepCommand regulatingStepCommand)
        {
            ASdu aSdu = new ASdu(TypeId.C_RC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { regulatingStepCommand } }) });

            send(aSdu);
        }

        public void regulatingStepCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeRegulatingStepCommand regulatingStepCommand, IeTime56 timeTag)
        {
            ASdu aSdu = new ASdu(TypeId.C_RC_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { regulatingStepCommand, timeTag } }) });

            send(aSdu);
        }

        public void setNormalizedValueCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeNormalizedValue normalizedValue, IeQualifierOfSetPointCommand qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.C_SE_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { normalizedValue, qualifier } }) });

            send(aSdu);
        }

        public void setNormalizedValueCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeNormalizedValue normalizedValue, IeQualifierOfSetPointCommand qualifier,
            IeTime56 timeTag)
        {
            ASdu aSdu = new ASdu(TypeId.C_SE_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { normalizedValue, qualifier, timeTag } }) });

            send(aSdu);
        }

        public void setScaledValueCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeScaledValue scaledValue, IeQualifierOfSetPointCommand qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.C_SE_NB_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { scaledValue, qualifier } }) });

            send(aSdu);
        }

        public void setScaledValueCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeScaledValue scaledValue, IeQualifierOfSetPointCommand qualifier,
            IeTime56 timeTag)
        {
            ASdu aSdu = new ASdu(TypeId.C_SE_TB_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { scaledValue, qualifier, timeTag } }) });

            send(aSdu);
        }

        public void setShortFloatCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeShortFloat shortFloat, IeQualifierOfSetPointCommand qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.C_SE_NC_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { shortFloat, qualifier } }) });

            send(aSdu);
        }

        public void setShortFloatCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeShortFloat shortFloat, IeQualifierOfSetPointCommand qualifier,
            IeTime56 timeTag)
        {
            ASdu aSdu = new ASdu(TypeId.C_SE_TC_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { shortFloat, qualifier, timeTag } }) });

            send(aSdu);
        }

        public void bitStringCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeBinaryStateInformation binaryStateInformation)
        {
            ASdu aSdu = new ASdu(TypeId.C_BO_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { binaryStateInformation } }) });

            send(aSdu);
        }

        public void bitStringCommandWithTimeTag(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeBinaryStateInformation binaryStateInformation, IeTime56 timeTag)
        {
            ASdu aSdu = new ASdu(TypeId.C_BO_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { binaryStateInformation, timeTag } }) });

            send(aSdu);
        }

        public void interrogation(int commonAddress, CauseOfTransmission cot, IeQualifierOfInterrogation qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.C_IC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] { new InformationObject(0, new InformationElement[][] { new InformationElement[] { qualifier } }) });

            send(aSdu);
        }

        public void counterInterrogation(int commonAddress, CauseOfTransmission cot,
            IeQualifierOfCounterInterrogation qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.C_CI_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] { new InformationObject(0, new InformationElement[][] { new InformationElement[] { qualifier } }) });

            send(aSdu);
        }

        public void readCommand(int commonAddress, int informationObjectAddress)
        {
            ASdu aSdu = new ASdu(TypeId.C_RD_NA_1, false, CauseOfTransmission.REQUEST, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { }) });

            send(aSdu);
        }

        public void synchronizeClocks(int commonAddress, IeTime56 time)
        {
            InformationObject io = new InformationObject(0, new InformationElement[][] { new InformationElement[] { time } });

            InformationObject[] ios = new InformationObject[] { io };

            ASdu aSdu = new ASdu(TypeId.C_CS_NA_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, ios);

            send(aSdu);
        }

        public void testCommand(int commonAddress)
        {
            ASdu aSdu = new ASdu(TypeId.C_TS_NA_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(0,
                new InformationElement[][] { new InformationElement[] { new IeFixedTestBitPattern() } }) });

            send(aSdu);
        }

        public void testCommandWithTimeTag(int commonAddress, IeTestSequenceCounter testSequenceCounter, IeTime56 time)
        {
            ASdu aSdu = new ASdu(TypeId.C_TS_TA_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(0, new InformationElement[][] { new InformationElement[] { testSequenceCounter, time } }) });

            send(aSdu);
        }

        public void resetProcessCommand(int commonAddress, IeQualifierOfResetProcessCommand qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.C_RP_NA_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(0,
                new InformationElement[][] { new InformationElement[] { qualifier } }) });

            send(aSdu);
        }

        public void delayAcquisitionCommand(int commonAddress, CauseOfTransmission cot, IeTime16 time)
        {
            ASdu aSdu = new ASdu(TypeId.C_CD_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] { new InformationObject(0, new InformationElement[][] { new InformationElement[] { time } }) });

            send(aSdu);
        }

        public void parameterNormalizedValueCommand(int commonAddress, int informationObjectAddress,
            IeNormalizedValue normalizedValue, IeQualifierOfParameterOfMeasuredValues qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.P_ME_NA_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { normalizedValue, qualifier } }) });

            send(aSdu);
        }

        public void parameterScaledValueCommand(int commonAddress, int informationObjectAddress, IeScaledValue scaledValue,
            IeQualifierOfParameterOfMeasuredValues qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.P_ME_NB_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { scaledValue, qualifier } }) });

            send(aSdu);
        }

        public void parameterShortFloatCommand(int commonAddress, int informationObjectAddress, IeShortFloat shortFloat,
            IeQualifierOfParameterOfMeasuredValues qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.P_ME_NC_1, false, CauseOfTransmission.ACTIVATION, false, false, originatorAddress,
                commonAddress, new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { shortFloat, qualifier } }) });

            send(aSdu);
        }

        public void parameterActivation(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeQualifierOfParameterActivation qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.P_AC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { qualifier } }) });

            send(aSdu);
        }

        public void fileReady(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
                IeLengthOfFileOrSection lengthOfFile, IeFileReadyQualifier qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.F_FR_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new InformationObject[] {
                new InformationObject( informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, lengthOfFile, qualifier } }) });

            send(aSdu);
        }

        public void sectionReady(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
                IeNameOfSection nameOfSection, IeLengthOfFileOrSection lengthOfSection, IeSectionReadyQualifier qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.F_SR_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new InformationObject[] {
                new InformationObject(
                informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, nameOfSection, lengthOfSection, qualifier } }) });

            send(aSdu);
        }

        public void callOrSelectFiles(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
                IeNameOfFile nameOfFile, IeNameOfSection nameOfSection, IeSelectAndCallQualifier qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.F_SC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, nameOfSection, qualifier } }) });

            send(aSdu);
        }

        public void lastSectionOrSegment(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
                IeNameOfSection nameOfSection, IeLastSectionOrSegmentQualifier qualifier, IeChecksum checksum)
        {
            ASdu aSdu = new ASdu(TypeId.F_LS_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new InformationObject[] {
                new InformationObject(
                informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, nameOfSection, qualifier, checksum } }) });

            send(aSdu);
        }

        public void ackFileOrSection(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
                IeNameOfSection nameOfSection, IeAckFileOrSectionQualifier qualifier)
        {
            ASdu aSdu = new ASdu(TypeId.F_AF_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new InformationObject[] {
                new InformationObject(
                informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, nameOfSection, qualifier } }) });

            send(aSdu);
        }

        public void sendSegment(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
                IeNameOfSection nameOfSection, IeFileSegment segment)
        {
            ASdu aSdu = new ASdu(TypeId.F_SG_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress,
                new InformationObject[] {
                new InformationObject(informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, nameOfSection, segment } }) });
            send(aSdu);
        }

        public void sendDirectory(int commonAddress, int informationObjectAddress, InformationElement[][] directory)
        {
            ASdu aSdu = new ASdu(TypeId.F_DR_TA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new InformationObject[] {
                new InformationObject(
                informationObjectAddress, directory) });

            send(aSdu);
        }

        public void queryLog(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
                IeTime56 rangeStartTime, IeTime56 rangeEndTime)
        {
            ASdu aSdu = new ASdu(TypeId.F_SC_NB_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new InformationObject[] {
                new InformationObject(
                informationObjectAddress,
                new InformationElement[][] { new InformationElement[] { nameOfFile, rangeStartTime, rangeEndTime } }) });

            send(aSdu);
        }

        public void setOriginatorAddress(int originatorAddress)
        {
            if (originatorAddress < 0 || originatorAddress > 255)
            {
                throw new ArgumentException("Originator Address must be between 0 and 255.");
            }

            this.originatorAddress = originatorAddress;
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
            lock (this)
            {
                return getSequenceNumberDifference(sendSequenceNumber, acknowledgedSendSequenceNumber);
            }
        }

        public int getOriginatorAddress()
        {
            return originatorAddress;
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

                if (sendSequenceNumber != acknowledgedSendSequenceNumber)
                {
                    scheduleMaxTimeNoAckReceivedFuture();
                }
            }
        }

        private void resetMaxIdleTimeTimer()
        {
            if (maxIdleTimeTimerFuture != null)
            {
                maxIdleTimeTimerFuture.Cancel();
                maxIdleTimeTimerFuture = null;
            }

            maxIdleTimeTimerFuture = PeriodicTaskFactory.Start(() =>
            {
                try
                {
                    writer.Write(TESTFR_ACT_BUFFER, 0, TESTFR_ACT_BUFFER.Length);
                    writer.Flush();
                }
                catch (Exception)
                {
                }
                scheduleMaxTimeNoTestConReceivedFuture();
            }, settings.maxIdleTime);
        }

        private void scheduleMaxTimeNoTestConReceivedFuture()
        {
            if (maxTimeNoTestConReceivedFuture != null)
            {
                maxTimeNoTestConReceivedFuture.Cancel();
                maxTimeNoTestConReceivedFuture = null;
            }

            maxTimeNoTestConReceivedFuture = PeriodicTaskFactory.Start(() =>
            {
                close();                
                if (connectionClosed != null)
                {
                    connectionClosed(new IOException(
                            "The maximum time that no test frame confirmation was received (t1) has been exceeded. t1 = "
                                    + settings.maxTimeNoAckReceived + "ms"));
                }

            }, settings.maxTimeNoAckReceived);
        }

        private void scheduleMaxTimeNoAckReceivedFuture()
        {
            if (maxTimeNoAckReceivedFuture != null)
            {
                maxTimeNoAckReceivedFuture.Cancel();
                maxTimeNoAckReceivedFuture = null;
            }

            maxTimeNoAckReceivedFuture = PeriodicTaskFactory.Start(() =>
            {
                close();
                maxTimeNoTestConReceivedFuture = null;
                if (connectionClosed != null)
                {
                    connectionClosed(new IOException(
                            "The maximum time that no test frame confirmation was received (t1) has been exceeded. t1 = "
                                    + settings.maxTimeNoAckReceived + "ms"));
                }

            }, settings.maxTimeNoAckReceived);
        }
    }
}
