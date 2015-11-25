using IEC60870.Connection;
using IEC60870.Enum;
using System;
using System.IO;
using System.Text;

namespace IEC60870.Object
{
    public class ASdu
    {
        private TypeId typeId;

        public Boolean isSequenceOfElements;

        private CauseOfTransmission causeOfTransmission;
        private Boolean test;
        private Boolean negativeConfirm;
        private int originatorAddress;
        private int commonAddress;
        private InformationObject[] informationObjects;
        private byte[] privateInformation;
        private int sequenceLength;

        public ASdu(TypeId typeId, Boolean isSequenceOfElements, CauseOfTransmission causeOfTransmission, Boolean test,
            Boolean negativeConfirm, int originatorAddress, int commonAddress, InformationObject[] informationObjects)
        {
            this.typeId = typeId;
            this.isSequenceOfElements = isSequenceOfElements;
            this.causeOfTransmission = causeOfTransmission;
            this.test = test;
            this.negativeConfirm = negativeConfirm;
            this.originatorAddress = originatorAddress;
            this.commonAddress = commonAddress;
            this.informationObjects = informationObjects;

            privateInformation = null;
            if (isSequenceOfElements)
            {
                sequenceLength = informationObjects[0].getInformationElements().Length;
            }
            else
            {
                sequenceLength = informationObjects.Length;
            }
        }

        public ASdu(TypeId typeId, Boolean isSequenceOfElements, int sequenceLength,
            CauseOfTransmission causeOfTransmission, Boolean test, Boolean negativeConfirm, int originatorAddress,
            int commonAddress, byte[] privateInformation)
        {

            this.typeId = typeId;
            this.isSequenceOfElements = isSequenceOfElements;
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

            typeId = (TypeId)typeIdCode;

            int tempbyte = reader.ReadByte();

            isSequenceOfElements = (tempbyte & 0x80) == 0x80;

            int numberOfSequenceElements;
            int numberOfInformationObjects;

            sequenceLength = tempbyte & 0x7f;
            if (isSequenceOfElements)
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
            causeOfTransmission = (CauseOfTransmission)(tempbyte & 0x3f);
            test = (tempbyte & 0x80) == 0x80;
            negativeConfirm = (tempbyte & 0x40) == 0x40;

            if (settings.cotFieldLength == 2)
            {
                originatorAddress = reader.ReadByte();
                aSduLength--;
            }
            else
            {
                originatorAddress = -1;
            }

            if (settings.commonAddressFieldLength == 1)
            {
                commonAddress = reader.ReadByte();
            }
            else
            {
                commonAddress = reader.ReadByte() + ((reader.ReadByte() << 8));
                aSduLength--;
            }

            if (typeIdCode < 128)
            {                
                informationObjects = new InformationObject[numberOfInformationObjects];

                for (int i = 0; i < numberOfInformationObjects; i++)
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

        public TypeId getTypeIdentification()
        {
            return typeId;
        }

        public int getSequenceLength()
        {
            return sequenceLength;
        }

        public CauseOfTransmission getCauseOfTransmission()
        {
            return causeOfTransmission;
        }

        public Boolean isTestFrame()
        {
            return test;
        }

        public Boolean isNegativeConfirm()
        {
            return negativeConfirm;
        }

        public int getOriginatorAddress()
        {
            return originatorAddress;
        }

        public int getCommonAddress()
        {
            return commonAddress;
        }

        public InformationObject[] getInformationObjects()
        {
            return informationObjects;
        }

        public byte[] getPrivateInformation()
        {
            return privateInformation;
        }

        public int encode(byte[] buffer, int i, ConnectionSettings settings)
        {
            int origi = i;

            buffer[i++] = (byte)typeId;
            if (isSequenceOfElements)
            {
                buffer[i++] = (byte)(sequenceLength | 0x80);
            }
            else
            {
                buffer[i++] = (byte)sequenceLength;
            }

            if (test)
            {
                if (negativeConfirm)
                {
                    buffer[i++] = (byte)((byte)causeOfTransmission | 0xC0);
                }
                else
                {
                    buffer[i++] = (byte)((byte)causeOfTransmission | 0x80);
                }
            }
            else
            {
                if (negativeConfirm)
                {
                    buffer[i++] = (byte)((byte)causeOfTransmission | 0x40);
                }
                else
                {
                    buffer[i++] = (byte)causeOfTransmission;
                }
            }

            if (settings.cotFieldLength == 2)
            {
                buffer[i++] = (byte)originatorAddress;
            }

            buffer[i++] = (byte)commonAddress;

            if (settings.commonAddressFieldLength == 2)
            {
                buffer[i++] = (byte)(commonAddress >> 8);
            }

            if (informationObjects != null)
            {
                foreach (InformationObject informationObject in informationObjects)
                {
                    i += informationObject.encode(buffer, i, settings);
                }
            }
            else
            {
                Array.Copy(privateInformation, 0, buffer, i, privateInformation.Length);
                i += privateInformation.Length;
            }

            return i - origi;
        }

        public override String ToString()
        {
            StringBuilder builder = new StringBuilder("Type ID: " + (int)typeId + "\nCause of transmission: " + causeOfTransmission + ", test: "
                    + isTestFrame() + ", negative con: " + isNegativeConfirm() + "\nOriginator address: "
                    + originatorAddress + ", Common address: " + commonAddress);

            if (informationObjects != null)
            {
                foreach (InformationObject informationObject in informationObjects)
                {
                    builder.Append("\n");
                    builder.Append(informationObject.ToString());
                }
            }
            else
            {
                builder.Append("\nPrivate Information:\n");
                int l = 1;
                foreach (byte b in privateInformation)
                {
                    if ((l != 1) && ((l - 1) % 8 == 0))
                    {
                        builder.Append(' ');
                    }
                    if ((l != 1) && ((l - 1) % 16 == 0))
                    {
                        builder.Append('\n');
                    }
                    l++;
                    builder.Append("0x");
                    String hexString = (b & 0xff).ToString("X");
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
