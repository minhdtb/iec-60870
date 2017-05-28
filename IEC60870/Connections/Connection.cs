using System;
using System.IO;
using System.Net.Sockets;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;
using IEC60870.Object;
using IEC60870.Utils;

namespace IEC60870.Connections
{
    public class Connection
    {
        private static readonly byte[] TestfrActBuffer = {0x68, 0x04, 0x43, 0x00, 0x00, 0x00};
        private static readonly byte[] TestfrConBuffer = {0x68, 0x04, 0x83, 0x00, 0x00, 0x00};
        private static readonly byte[] StartdtActBuffer = {0x68, 0x04, 0x07, 0x00, 0x00, 0x00};
        private static readonly byte[] StartdtConBuffer = {0x68, 0x04, 0x0b, 0x00, 0x00, 0x00};

        private readonly byte[] buffer = new byte[255];
        private readonly BinaryReader reader;

        private readonly ConnectionSettings settings;

        private readonly BinaryWriter writer;

        private bool closed;
        private IOException closedIoException;
        public ConnectionEventListener.ConnectionClosed ConnectionClosed = null;

        private RunTask maxIdleTimeTimerFuture;

        private RunTask maxTimeNoAckReceivedFuture;

        private RunTask maxTimeNoAckSentFuture;

        private RunTask maxTimeNoTestConReceivedFuture;

        public ConnectionEventListener.NewASdu NewASdu = null;

        private int originatorAddress;

        private int receiveSequenceNumber;
        private int sendSequenceNumber;

        private int acknowledgedReceiveSequenceNumber;
        private int acknowledgedSendSequenceNumber;

        private readonly CountDownLatch startdtactSignal;
        private CountDownLatch startdtConSignal;

        public Connection(Socket socket, ConnectionSettings settings)
        {
            this.settings = settings;

            var ns = new NetworkStream(socket);

            writer = new BinaryWriter(ns);
            reader = new BinaryReader(ns);

            startdtactSignal = new CountDownLatch(1);

            var connectionReader = new ConnectionReader(this);
            connectionReader.Start();
        }

        public void Close()
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

        public void StartDataTransfer(int timeout = 0)
        {
            if (timeout < 0)
            {
                throw new ArgumentException("timeout may not be negative");
            }

            startdtConSignal = new CountDownLatch(1);

            try
            {
                writer.Write(StartdtActBuffer, 0, StartdtActBuffer.Length);
                writer.Flush();
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }

            if (timeout == 0)
            {
                startdtConSignal.Wait();
            }
            else
            {
                startdtConSignal.Wait(timeout);
            }
        }

        public void WaitForStartDT(int timeout = 0)
        {
            if (timeout < 0)
            {
                throw new ArgumentException("timeout may not be negative");
            }

            if (timeout == 0)
            {
                startdtactSignal.Wait();
            }
            else
            {
                startdtactSignal.Wait(timeout);
            }

            try
            {
                writer.Write(StartdtConBuffer, 0, StartdtConBuffer.Length);
                writer.Flush();
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }

            ResetMaxIdleTimeTimer();
        }

        public void Send(ASdu aSdu)
        {
            acknowledgedReceiveSequenceNumber = receiveSequenceNumber;
            var requestAPdu = new APdu(sendSequenceNumber, receiveSequenceNumber, APdu.ApciType.I_FORMAT, aSdu);
            sendSequenceNumber = (sendSequenceNumber + 1) % 32768;

            if (maxTimeNoAckSentFuture != null)
            {
                maxTimeNoAckSentFuture.Cancel();
                maxTimeNoAckSentFuture = null;
            }

            if (maxTimeNoAckReceivedFuture == null)
            {
                ScheduleMaxTimeNoAckReceivedFuture();
            }

            var length = requestAPdu.Encode(buffer, settings);
            writer.Write(buffer, 0, length);
            writer.Flush();

            ResetMaxIdleTimeTimer();
        }

        private void SendSFormatPdu()
        {
            var requestAPdu = new APdu(0, receiveSequenceNumber, APdu.ApciType.S_FORMAT, null);
            requestAPdu.Encode(buffer, settings);

            writer.Write(buffer, 0, 6);
            writer.Flush();

            acknowledgedReceiveSequenceNumber = receiveSequenceNumber;

            ResetMaxIdleTimeTimer();
        }

        #region COMMANDS

        public void SendConfirmation(ASdu aSdu)
        {
            var cot = aSdu.GetCauseOfTransmission();

            if (cot == CauseOfTransmission.ACTIVATION)
            {
                cot = CauseOfTransmission.ACTIVATION_CON;
            }
            else if (cot == CauseOfTransmission.DEACTIVATION)
            {
                cot = CauseOfTransmission.DEACTIVATION_CON;
            }

            Send(new ASdu(aSdu.GetTypeIdentification(), aSdu.IsSequenceOfElements, cot, aSdu.IsTestFrame(),
                aSdu.IsNegativeConfirm(), aSdu.GetOriginatorAddress(), aSdu.GetCommonAddress(),
                aSdu.GetInformationObjects()));
        }

        public void SingleCommand(int commonAddress, int informationObjectAddress, IeSingleCommand singleCommand)
        {
            var cot = singleCommand.IsCommandStateOn()
                ? CauseOfTransmission.ACTIVATION
                : CauseOfTransmission.DEACTIVATION;

            var aSdu = new ASdu(TypeId.C_SC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {singleCommand}})
                });

            Send(aSdu);
        }

        public void SingleCommandWithTimeTag(int commonAddress, int informationObjectAddress,
            IeSingleCommand singleCommand, IeTime56 timeTag)
        {
            var cot = singleCommand.IsCommandStateOn()
                ? CauseOfTransmission.ACTIVATION
                : CauseOfTransmission.DEACTIVATION;

            var aSdu = new ASdu(TypeId.C_SC_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {singleCommand, timeTag}})
                });

            Send(aSdu);
        }

        public void DoubleCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeDoubleCommand doubleCommand)
        {
            var aSdu = new ASdu(TypeId.C_DC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {doubleCommand}})
                });

            Send(aSdu);
        }

        public void DoubleCommandWithTimeTag(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeDoubleCommand doubleCommand, IeTime56 timeTag)
        {
            var aSdu = new ASdu(TypeId.C_DC_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {doubleCommand, timeTag}})
                });

            Send(aSdu);
        }

        public void RegulatingStepCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeRegulatingStepCommand regulatingStepCommand)
        {
            var aSdu = new ASdu(TypeId.C_RC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {regulatingStepCommand}})
                });

            Send(aSdu);
        }

        public void RegulatingStepCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeRegulatingStepCommand regulatingStepCommand, IeTime56 timeTag)
        {
            var aSdu = new ASdu(TypeId.C_RC_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {regulatingStepCommand, timeTag}})
                });

            Send(aSdu);
        }

        public void SetNormalizedValueCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeNormalizedValue normalizedValue, IeQualifierOfSetPointCommand qualifier)
        {
            var aSdu = new ASdu(TypeId.C_SE_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {normalizedValue, qualifier}})
                });

            Send(aSdu);
        }

        public void SetNormalizedValueCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeNormalizedValue normalizedValue, IeQualifierOfSetPointCommand qualifier,
            IeTime56 timeTag)
        {
            var aSdu = new ASdu(TypeId.C_SE_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {normalizedValue, qualifier, timeTag}})
                });

            Send(aSdu);
        }

        public void SetScaledValueCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeScaledValue scaledValue, IeQualifierOfSetPointCommand qualifier)
        {
            var aSdu = new ASdu(TypeId.C_SE_NB_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {scaledValue, qualifier}})
                });

            Send(aSdu);
        }

        public void SetScaledValueCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeScaledValue scaledValue, IeQualifierOfSetPointCommand qualifier,
            IeTime56 timeTag)
        {
            var aSdu = new ASdu(TypeId.C_SE_TB_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {scaledValue, qualifier, timeTag}})
                });

            Send(aSdu);
        }

        public void SetShortFloatCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeShortFloat shortFloat, IeQualifierOfSetPointCommand qualifier)
        {
            var aSdu = new ASdu(TypeId.C_SE_NC_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {shortFloat, qualifier}})
                });

            Send(aSdu);
        }

        public void SetShortFloatCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress, IeShortFloat shortFloat, IeQualifierOfSetPointCommand qualifier,
            IeTime56 timeTag)
        {
            var aSdu = new ASdu(TypeId.C_SE_TC_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {shortFloat, qualifier, timeTag}})
                });

            Send(aSdu);
        }

        public void BitStringCommand(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeBinaryStateInformation binaryStateInformation)
        {
            var aSdu = new ASdu(TypeId.C_BO_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {binaryStateInformation}})
                });

            Send(aSdu);
        }

        public void BitStringCommandWithTimeTag(int commonAddress, CauseOfTransmission cot,
            int informationObjectAddress,
            IeBinaryStateInformation binaryStateInformation, IeTime56 timeTag)
        {
            var aSdu = new ASdu(TypeId.C_BO_TA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {binaryStateInformation, timeTag}})
                });

            Send(aSdu);
        }

        public void Interrogation(int commonAddress, CauseOfTransmission cot, IeQualifierOfInterrogation qualifier)
        {
            var aSdu = new ASdu(TypeId.C_IC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[] {new InformationObject(0, new[] {new InformationElement[] {qualifier}})});

            Send(aSdu);
        }

        public void CounterInterrogation(int commonAddress, CauseOfTransmission cot,
            IeQualifierOfCounterInterrogation qualifier)
        {
            var aSdu = new ASdu(TypeId.C_CI_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[] {new InformationObject(0, new[] {new InformationElement[] {qualifier}})});

            Send(aSdu);
        }

        public void ReadCommand(int commonAddress, int informationObjectAddress)
        {
            var aSdu = new ASdu(TypeId.C_RD_NA_1, false, CauseOfTransmission.REQUEST, false, false, originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(informationObjectAddress,
                        new InformationElement[][] { })
                });

            Send(aSdu);
        }

        public void SynchronizeClocks(int commonAddress, IeTime56 time)
        {
            var io = new InformationObject(0, new[] {new InformationElement[] {time}});

            InformationObject[] ios = {io};

            var aSdu = new ASdu(TypeId.C_CS_NA_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, ios);

            Send(aSdu);
        }

        public void TestCommand(int commonAddress)
        {
            var aSdu = new ASdu(TypeId.C_TS_NA_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(0,
                        new[] {new InformationElement[] {new IeFixedTestBitPattern()}})
                });

            Send(aSdu);
        }

        public void TestCommandWithTimeTag(int commonAddress, IeTestSequenceCounter testSequenceCounter, IeTime56 time)
        {
            var aSdu = new ASdu(TypeId.C_TS_TA_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(0, new[] {new InformationElement[] {testSequenceCounter, time}})
                });

            Send(aSdu);
        }

        public void ResetProcessCommand(int commonAddress, IeQualifierOfResetProcessCommand qualifier)
        {
            var aSdu = new ASdu(TypeId.C_RP_NA_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(0,
                        new[] {new InformationElement[] {qualifier}})
                });

            Send(aSdu);
        }

        public void DelayAcquisitionCommand(int commonAddress, CauseOfTransmission cot, IeTime16 time)
        {
            var aSdu = new ASdu(TypeId.C_CD_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[] {new InformationObject(0, new[] {new InformationElement[] {time}})});

            Send(aSdu);
        }

        public void ParameterNormalizedValueCommand(int commonAddress, int informationObjectAddress,
            IeNormalizedValue normalizedValue, IeQualifierOfParameterOfMeasuredValues qualifier)
        {
            var aSdu = new ASdu(TypeId.P_ME_NA_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {normalizedValue, qualifier}})
                });

            Send(aSdu);
        }

        public void ParameterScaledValueCommand(int commonAddress, int informationObjectAddress,
            IeScaledValue scaledValue,
            IeQualifierOfParameterOfMeasuredValues qualifier)
        {
            var aSdu = new ASdu(TypeId.P_ME_NB_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {scaledValue, qualifier}})
                });

            Send(aSdu);
        }

        public void ParameterShortFloatCommand(int commonAddress, int informationObjectAddress, IeShortFloat shortFloat,
            IeQualifierOfParameterOfMeasuredValues qualifier)
        {
            var aSdu = new ASdu(TypeId.P_ME_NC_1, false, CauseOfTransmission.ACTIVATION, false, false,
                originatorAddress,
                commonAddress, new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {shortFloat, qualifier}})
                });

            Send(aSdu);
        }

        public void ParameterActivation(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeQualifierOfParameterActivation qualifier)
        {
            var aSdu = new ASdu(TypeId.P_AC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {qualifier}})
                });

            Send(aSdu);
        }

        public void FileReady(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
            IeLengthOfFileOrSection lengthOfFile, IeFileReadyQualifier qualifier)
        {
            var aSdu = new ASdu(TypeId.F_FR_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, lengthOfFile, qualifier}})
                });

            Send(aSdu);
        }

        public void SectionReady(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
            IeNameOfSection nameOfSection, IeLengthOfFileOrSection lengthOfSection, IeSectionReadyQualifier qualifier)
        {
            var aSdu = new ASdu(TypeId.F_SR_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new[]
                {
                    new InformationObject(
                        informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, nameOfSection, lengthOfSection, qualifier}})
                });

            Send(aSdu);
        }

        public void CallOrSelectFiles(int commonAddress, CauseOfTransmission cot, int informationObjectAddress,
            IeNameOfFile nameOfFile, IeNameOfSection nameOfSection, IeSelectAndCallQualifier qualifier)
        {
            var aSdu = new ASdu(TypeId.F_SC_NA_1, false, cot, false, false, originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, nameOfSection, qualifier}})
                });

            Send(aSdu);
        }

        public void LastSectionOrSegment(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
            IeNameOfSection nameOfSection, IeLastSectionOrSegmentQualifier qualifier, IeChecksum checksum)
        {
            var aSdu = new ASdu(TypeId.F_LS_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new[]
                {
                    new InformationObject(
                        informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, nameOfSection, qualifier, checksum}})
                });

            Send(aSdu);
        }

        public void AckFileOrSection(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
            IeNameOfSection nameOfSection, IeAckFileOrSectionQualifier qualifier)
        {
            var aSdu = new ASdu(TypeId.F_AF_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new[]
                {
                    new InformationObject(
                        informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, nameOfSection, qualifier}})
                });

            Send(aSdu);
        }

        public void SendSegment(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
            IeNameOfSection nameOfSection, IeFileSegment segment)
        {
            var aSdu = new ASdu(TypeId.F_SG_NA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress,
                new[]
                {
                    new InformationObject(informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, nameOfSection, segment}})
                });
            Send(aSdu);
        }

        public void SendDirectory(int commonAddress, int informationObjectAddress, InformationElement[][] directory)
        {
            var aSdu = new ASdu(TypeId.F_DR_TA_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new[]
                {
                    new InformationObject(
                        informationObjectAddress, directory)
                });

            Send(aSdu);
        }

        public void QueryLog(int commonAddress, int informationObjectAddress, IeNameOfFile nameOfFile,
            IeTime56 rangeStartTime, IeTime56 rangeEndTime)
        {
            var aSdu = new ASdu(TypeId.F_SC_NB_1, false, CauseOfTransmission.FILE_TRANSFER, false, false,
                originatorAddress, commonAddress, new[]
                {
                    new InformationObject(
                        informationObjectAddress,
                        new[] {new InformationElement[] {nameOfFile, rangeStartTime, rangeEndTime}})
                });

            Send(aSdu);
        }

        #endregion

        #region HELPER

        public void SetOriginatorAddress(int address)
        {
            if (address < 0 || address > 255)
            {
                throw new ArgumentException("Originator Address must be between 0 and 255.");
            }

            originatorAddress = address;
        }

        private int GetSequenceNumberDifference(int x, int y)
        {
            var difference = x - y;
            if (difference < 0)
            {
                difference += 32768;
            }

            return difference;
        }

        public int GetNumUnconfirmedIPdusSent()
        {
            lock (this)
            {
                return GetSequenceNumberDifference(sendSequenceNumber, acknowledgedSendSequenceNumber);
            }
        }

        public int GetOriginatorAddress()
        {
            return originatorAddress;
        }

        #endregion

        private void HandleReceiveSequenceNumber(APdu aPdu)
        {
            if (acknowledgedSendSequenceNumber != aPdu.GetReceiveSeqNumber())
            {
                if (GetSequenceNumberDifference(aPdu.GetReceiveSeqNumber(), acknowledgedSendSequenceNumber) >
                    GetNumUnconfirmedIPdusSent())
                {
                    throw new IOException("Got unexpected receive sequence number: " + aPdu.GetReceiveSeqNumber()
                                          + ", expected a number between: " + acknowledgedSendSequenceNumber + " and "
                                          + sendSequenceNumber);
                }

                if (maxTimeNoAckReceivedFuture != null)
                {
                    maxTimeNoAckReceivedFuture.Cancel();
                    maxTimeNoAckReceivedFuture = null;
                }

                acknowledgedSendSequenceNumber = aPdu.GetReceiveSeqNumber();

                if (sendSequenceNumber != acknowledgedSendSequenceNumber)
                {
                    ScheduleMaxTimeNoAckReceivedFuture();
                }
            }
        }

        private void ResetMaxIdleTimeTimer()
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
                    writer.Write(TestfrActBuffer, 0, TestfrActBuffer.Length);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    throw new IOException(e.Message);
                }

                ScheduleMaxTimeNoTestConReceivedFuture();
            }, settings.MaxIdleTime);
        }

        private void ScheduleMaxTimeNoTestConReceivedFuture()
        {
            if (maxTimeNoTestConReceivedFuture != null)
            {
                maxTimeNoTestConReceivedFuture.Cancel();
                maxTimeNoTestConReceivedFuture = null;
            }

            maxTimeNoTestConReceivedFuture = PeriodicTaskFactory.Start(() =>
            {
                Close();
                ConnectionClosed?.Invoke(new IOException(
                    "The maximum time that no test frame confirmation was received (t1) has been exceeded. t1 = "
                    + settings.MaxTimeNoAckReceived + "ms"));
            }, settings.MaxTimeNoAckReceived);
        }

        private void ScheduleMaxTimeNoAckReceivedFuture()
        {
            if (maxTimeNoAckReceivedFuture != null)
            {
                maxTimeNoAckReceivedFuture.Cancel();
                maxTimeNoAckReceivedFuture = null;
            }

            maxTimeNoAckReceivedFuture = PeriodicTaskFactory.Start(() =>
            {
                Close();
                maxTimeNoTestConReceivedFuture = null;
                ConnectionClosed?.Invoke(new IOException(
                    "The maximum time that no test frame confirmation was received (t1) has been exceeded. t1 = "
                    + settings.MaxTimeNoAckReceived + "ms"));
            }, settings.MaxTimeNoAckReceived);
        }

        private class ConnectionReader : ThreadBase
        {
            private readonly Connection innerConnection;

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

                        var aPdu = new APdu(reader, innerConnection.settings);
                        switch (aPdu.GetApciType())
                        {
                            case APdu.ApciType.I_FORMAT:
                                if (innerConnection.receiveSequenceNumber != aPdu.GetSendSeqNumber())
                                {
                                    throw new IOException("Got unexpected send sequence number: " +
                                                          aPdu.GetSendSeqNumber()
                                                          + ", expected: " + innerConnection.receiveSequenceNumber);
                                }

                                innerConnection.receiveSequenceNumber = (aPdu.GetSendSeqNumber() + 1) % 32768;
                                innerConnection.HandleReceiveSequenceNumber(aPdu);

                                innerConnection.NewASdu?.Invoke(aPdu.GetASdu());

                                var numUnconfirmedIPdusReceived = innerConnection.GetSequenceNumberDifference(
                                    innerConnection.receiveSequenceNumber,
                                    innerConnection.acknowledgedReceiveSequenceNumber);
                                if (numUnconfirmedIPdusReceived > innerConnection.settings.MaxUnconfirmedIPdusReceived)
                                {
                                    innerConnection.SendSFormatPdu();
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
                                        innerConnection.maxTimeNoAckSentFuture =
                                            PeriodicTaskFactory.Start(() =>
                                            {
                                                innerConnection.SendSFormatPdu();
                                                innerConnection.maxTimeNoAckSentFuture = null;
                                            }, innerConnection.settings.MaxTimeNoAckSent);
                                    }
                                }

                                innerConnection.ResetMaxIdleTimeTimer();
                                break;
                            case APdu.ApciType.STARTDT_CON:
                                innerConnection.startdtConSignal?.CountDown();
                                innerConnection.ResetMaxIdleTimeTimer();
                                break;
                            case APdu.ApciType.STARTDT_ACT:
                                innerConnection.startdtactSignal?.CountDown();
                                break;
                            case APdu.ApciType.S_FORMAT:
                                innerConnection.HandleReceiveSequenceNumber(aPdu);
                                innerConnection.ResetMaxIdleTimeTimer();
                                break;
                            case APdu.ApciType.TESTFR_ACT:
                                try
                                {
                                    innerConnection.writer.Write(TestfrConBuffer, 0, TestfrConBuffer.Length);
                                    innerConnection.writer.Flush();
                                }
                                catch (Exception e)
                                {
                                    throw new IOException(e.Message);
                                }

                                innerConnection.ResetMaxIdleTimeTimer();
                                break;
                            case APdu.ApciType.TESTFR_CON:
                                if (innerConnection.maxTimeNoTestConReceivedFuture != null)
                                {
                                    innerConnection.maxTimeNoTestConReceivedFuture.Cancel();
                                    innerConnection.maxTimeNoTestConReceivedFuture = null;
                                }
                                innerConnection.ResetMaxIdleTimeTimer();
                                break;
                            default:
                                throw new IOException("Got unexpected message with APCI Type: " + aPdu.GetApciType());
                        }
                    }
                }
                catch (IOException e)
                {
                    innerConnection.closedIoException = e;
                }
                catch (Exception e)
                {
                    innerConnection.closedIoException = new IOException(e.Message);
                }
                finally
                {
                    innerConnection.ConnectionClosed?.Invoke(innerConnection.closedIoException);
                    innerConnection.Close();
                }
            }
        }
    }
}