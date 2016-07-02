using System;
using System.IO;
using System.Linq;
using System.Text;
using IEC60870.Connections;
using IEC60870.Enum;

namespace IEC60870.Object
{
    public class ASdu
    {
        private readonly CauseOfTransmission causeOfTransmission;
        private readonly int commonAddress;
        private readonly InformationObject[] informationObjects;
        private readonly bool negativeConfirm;
        private readonly int originatorAddress;
        private readonly byte[] privateInformation;
        private readonly int sequenceLength;
        private readonly bool test;
        private readonly TypeId typeId;

        public bool IsSequenceOfElements;

        public ASdu(TypeId typeId, bool isSequenceOfElements, CauseOfTransmission causeOfTransmission, bool test,
            bool negativeConfirm, int originatorAddress, int commonAddress, InformationObject[] informationObjects)
        {
            IsSequenceOfElements = isSequenceOfElements;

            this.typeId = typeId;
            this.causeOfTransmission = causeOfTransmission;
            this.test = test;
            this.negativeConfirm = negativeConfirm;
            this.originatorAddress = originatorAddress;
            this.commonAddress = commonAddress;
            this.informationObjects = informationObjects;

            privateInformation = null;

            sequenceLength = isSequenceOfElements
                ? informationObjects[0].GetInformationElements().Length
                : informationObjects.Length;
        }

        public ASdu(TypeId typeId, bool isSequenceOfElements, int sequenceLength,
            CauseOfTransmission causeOfTransmission, bool test, bool negativeConfirm, int originatorAddress,
            int commonAddress, byte[] privateInformation)
        {
            this.typeId = typeId;
            IsSequenceOfElements = isSequenceOfElements;
            this.causeOfTransmission = causeOfTransmission;
            this.test = test;
            this.negativeConfirm = negativeConfirm;
            this.originatorAddress = originatorAddress;
            this.commonAddress = commonAddress;
            informationObjects = null;
            this.privateInformation = privateInformation;
            this.sequenceLength = sequenceLength;
        }

        public ASdu(BinaryReader reader, ConnectionSettings settings, int aSduLength)
        {
            int typeIdCode = reader.ReadByte();

            typeId = (TypeId) typeIdCode;

            int tempbyte = reader.ReadByte();

            IsSequenceOfElements = (tempbyte & 0x80) == 0x80;

            int numberOfSequenceElements;
            int numberOfInformationObjects;

            sequenceLength = tempbyte & 0x7f;
            if (IsSequenceOfElements)
            {
                numberOfSequenceElements = sequenceLength;
                numberOfInformationObjects = 1;
            }
            else
            {
                numberOfInformationObjects = sequenceLength;
                numberOfSequenceElements = 1;
            }

            tempbyte = reader.ReadByte();
            causeOfTransmission = (CauseOfTransmission) (tempbyte & 0x3f);
            test = (tempbyte & 0x80) == 0x80;
            negativeConfirm = (tempbyte & 0x40) == 0x40;

            if (settings.CotFieldLength == 2)
            {
                originatorAddress = reader.ReadByte();
                aSduLength--;
            }
            else
            {
                originatorAddress = -1;
            }

            if (settings.CommonAddressFieldLength == 1)
            {
                commonAddress = reader.ReadByte();
            }
            else
            {
                commonAddress = reader.ReadByte() + (reader.ReadByte() << 8);
                aSduLength--;
            }

            if (typeIdCode < 128)
            {
                informationObjects = new InformationObject[numberOfInformationObjects];

                for (var i = 0; i < numberOfInformationObjects; i++)
                {
                    informationObjects[i] = new InformationObject(reader, typeId, numberOfSequenceElements, settings);
                }

                privateInformation = null;
            }
            else
            {
                informationObjects = null;
                privateInformation = reader.ReadBytes(aSduLength - 4);
            }
        }

        public TypeId GetTypeIdentification()
        {
            return typeId;
        }

        public int GetSequenceLength()
        {
            return sequenceLength;
        }

        public CauseOfTransmission GetCauseOfTransmission()
        {
            return causeOfTransmission;
        }

        public bool IsTestFrame()
        {
            return test;
        }

        public bool IsNegativeConfirm()
        {
            return negativeConfirm;
        }

        public int GetOriginatorAddress()
        {
            return originatorAddress;
        }

        public int GetCommonAddress()
        {
            return commonAddress;
        }

        public InformationObject[] GetInformationObjects()
        {
            return informationObjects;
        }

        public byte[] GetPrivateInformation()
        {
            return privateInformation;
        }

        public int Encode(byte[] buffer, int i, ConnectionSettings settings)
        {
            var origi = i;

            buffer[i++] = (byte) typeId;
            if (IsSequenceOfElements)
            {
                buffer[i++] = (byte) (sequenceLength | 0x80);
            }
            else
            {
                buffer[i++] = (byte) sequenceLength;
            }

            if (test)
            {
                if (negativeConfirm)
                {
                    buffer[i++] = (byte) ((byte) causeOfTransmission | 0xC0);
                }
                else
                {
                    buffer[i++] = (byte) ((byte) causeOfTransmission | 0x80);
                }
            }
            else
            {
                if (negativeConfirm)
                {
                    buffer[i++] = (byte) ((byte) causeOfTransmission | 0x40);
                }
                else
                {
                    buffer[i++] = (byte) causeOfTransmission;
                }
            }

            if (settings.CotFieldLength == 2)
            {
                buffer[i++] = (byte) originatorAddress;
            }

            buffer[i++] = (byte) commonAddress;

            if (settings.CommonAddressFieldLength == 2)
            {
                buffer[i++] = (byte) (commonAddress >> 8);
            }

            if (informationObjects != null)
            {
                i = informationObjects.Aggregate(i,
                    (current, informationObject) => current + informationObject.Encode(buffer, current, settings));
            }
            else
            {
                Array.Copy(privateInformation, 0, buffer, i, privateInformation.Length);
                i += privateInformation.Length;
            }

            return i - origi + 1;
        }

        public override string ToString()
        {
            var builder = new StringBuilder("Type ID: " + (int) typeId + ", " + Description.GetAttr(typeId).Name +
                                            "\nCause of transmission: " + causeOfTransmission + ", test: "
                                            + IsTestFrame() + ", negative con: " + IsNegativeConfirm() +
                                            "\nOriginator address: "
                                            + originatorAddress + ", Common address: " + commonAddress);

            if (informationObjects != null)
            {
                foreach (var informationObject in informationObjects)
                {
                    builder.Append("\n");
                    builder.Append(informationObject);
                }
            }
            else
            {
                builder.Append("\nPrivate Information:\n");
                var l = 1;
                foreach (var b in privateInformation)
                {
                    if ((l != 1) && ((l - 1)%8 == 0))
                    {
                        builder.Append(' ');
                    }
                    if ((l != 1) && ((l - 1)%16 == 0))
                    {
                        builder.Append('\n');
                    }
                    l++;
                    builder.Append("0x");
                    var hexString = (b & 0xff).ToString("X");
                    if (hexString.Length == 1)
                    {
                        builder.Append(0);
                    }
                    builder.Append(hexString + " ");
                }
            }

            return builder.ToString();
        }
    }
}
