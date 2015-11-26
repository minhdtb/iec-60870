using System.IO;
using System.Linq;
using System.Text;
using IEC60870.Connection;
using IEC60870.Enum;
using IEC60870.IE;
using IEC60870.IE.Base;

namespace IEC60870.Object
{
    public class InformationObject
    {
        private readonly InformationElement[][] informationElements;
        private readonly int informationObjectAddress;

        public InformationObject(int informationObjectAddress, InformationElement[][] informationElements)
        {
            this.informationObjectAddress = informationObjectAddress;
            this.informationElements = informationElements;
        }

        public InformationObject(BinaryReader reader, TypeId typeId, int numberOfSequenceElements,
            ConnectionSettings settings)
        {
            if (settings.IoaFieldLength == 1)
            {
                informationObjectAddress = reader.ReadByte();
            }
            else if (settings.IoaFieldLength == 2)
            {
                informationObjectAddress = reader.ReadByte() + (reader.ReadByte() << 8);
            }
            else if (settings.IoaFieldLength == 3)
            {
                informationObjectAddress = reader.ReadByte() + (reader.ReadByte() << 8) + (reader.ReadByte() << 16);
            }

            switch (typeId)
            {
                // 1
                case TypeId.M_SP_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[1];
                        informationElements[i][0] = new IeSinglePointWithQuality(reader);
                    }
                    break;
                // 2
                case TypeId.M_SP_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeSinglePointWithQuality(reader), new IeTime24(reader)}};
                    break;
                // 3
                case TypeId.M_DP_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[1];
                        informationElements[i][0] = new IeDoublePointWithQuality(reader);
                    }
                    break;
                // 4
                case TypeId.M_DP_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeDoublePointWithQuality(reader), new IeTime24(reader)}};
                    break;
                // 5
                case TypeId.M_ST_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[2];
                        informationElements[i][0] = new IeValueWithTransientState(reader);
                        informationElements[i][1] = new IeQuality(reader);
                    }
                    break;
                // 6
                case TypeId.M_ST_TA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeValueWithTransientState(reader), new IeQuality(reader), new IeTime24(reader)}
                    };
                    break;
                // 7
                case TypeId.M_BO_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[2];
                        informationElements[i][0] = new IeBinaryStateInformation(reader);
                        informationElements[i][1] = new IeQuality(reader);
                    }
                    break;
                // 8
                case TypeId.M_BO_TA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeBinaryStateInformation(reader), new IeQuality(reader), new IeTime24(reader)}
                    };
                    break;
                // 9
                case TypeId.M_ME_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[2];
                        informationElements[i][0] = new IeNormalizedValue(reader);
                        informationElements[i][1] = new IeQuality(reader);
                    }
                    break;
                // 10
                case TypeId.M_ME_TA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeNormalizedValue(reader), new IeQuality(reader), new IeTime24(reader)}
                    };
                    break;
                // 11
                case TypeId.M_ME_NB_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[2];
                        informationElements[i][0] = new IeScaledValue(reader);
                        informationElements[i][1] = new IeQuality(reader);
                    }
                    break;
                // 12
                case TypeId.M_ME_TB_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeScaledValue(reader), new IeQuality(reader), new IeTime24(reader)}};
                    break;
                // 13
                case TypeId.M_ME_NC_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[2];
                        informationElements[i][0] = new IeShortFloat(reader);
                        informationElements[i][1] = new IeQuality(reader);
                    }
                    break;
                // 14
                case TypeId.M_ME_TC_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeShortFloat(reader), new IeQuality(reader), new IeTime24(reader)}};
                    break;
                // 15
                case TypeId.M_IT_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[1];
                        informationElements[i][0] = new IeBinaryCounterReading(reader);
                    }
                    break;
                // 16
                case TypeId.M_IT_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeBinaryCounterReading(reader), new IeTime24(reader)}};
                    break;
                // 17
                case TypeId.M_EP_TA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeSingleProtectionEvent(reader), new IeTime16(reader), new IeTime24(reader)}
                    };
                    break;
                // 18
                case TypeId.M_EP_TB_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeProtectionStartEvent(reader),
                            new IeProtectionQuality(reader), new IeTime16(reader), new IeTime24(reader)
                        }
                    };
                    break;
                // 19
                case TypeId.M_EP_TC_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeProtectionOutputCircuitInformation(reader),
                            new IeProtectionQuality(reader), new IeTime16(reader), new IeTime24(reader)
                        }
                    };
                    break;
                // 20
                case TypeId.M_PS_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[2];
                        informationElements[i][0] = new IeStatusAndStatusChanges(reader);
                        informationElements[i][1] = new IeQuality(reader);
                    }
                    break;
                // 21
                case TypeId.M_ME_ND_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[1];
                        informationElements[i][0] = new IeNormalizedValue(reader);
                    }
                    break;
                // 30
                case TypeId.M_SP_TB_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeSinglePointWithQuality(reader), new IeTime56(reader)}};
                    break;
                // 31
                case TypeId.M_DP_TB_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeDoublePointWithQuality(reader), new IeTime56(reader)}};
                    break;
                // 32
                case TypeId.M_ST_TB_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeValueWithTransientState(reader), new IeQuality(reader), new IeTime56(reader)}
                    };
                    break;
                // 33
                case TypeId.M_BO_TB_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeBinaryStateInformation(reader), new IeQuality(reader), new IeTime56(reader)}
                    };
                    break;
                // 34
                case TypeId.M_ME_TD_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeNormalizedValue(reader), new IeQuality(reader), new IeTime56(reader)}
                    };
                    break;
                // 35
                case TypeId.M_ME_TE_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeScaledValue(reader), new IeQuality(reader), new IeTime56(reader)}};
                    break;
                // 36
                case TypeId.M_ME_TF_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeShortFloat(reader), new IeQuality(reader), new IeTime56(reader)}};
                    break;
                // 37
                case TypeId.M_IT_TB_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeBinaryCounterReading(reader), new IeTime56(reader)}};
                    break;
                // 38
                case TypeId.M_EP_TD_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {new IeSingleProtectionEvent(reader), new IeTime16(reader), new IeTime56(reader)}
                    };
                    break;
                // 39
                case TypeId.M_EP_TE_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeProtectionStartEvent(reader),
                            new IeProtectionQuality(reader), new IeTime16(reader), new IeTime56(reader)
                        }
                    };
                    break;
                // 40
                case TypeId.M_EP_TF_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeProtectionOutputCircuitInformation(reader),
                            new IeProtectionQuality(reader), new IeTime16(reader), new IeTime56(reader)
                        }
                    };
                    break;
                // 45
                case TypeId.C_SC_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeSingleCommand(reader)}};
                    break;
                // 46
                case TypeId.C_DC_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeDoubleCommand(reader)}};
                    break;
                // 47
                case TypeId.C_RC_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeRegulatingStepCommand(reader)}};
                    break;
                // 48
                case TypeId.C_SE_NA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeNormalizedValue(reader), new IeQualifierOfSetPointCommand(reader)}};
                    break;
                // 49
                case TypeId.C_SE_NB_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeScaledValue(reader), new IeQualifierOfSetPointCommand(reader)}};
                    break;
                // 50
                case TypeId.C_SE_NC_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeShortFloat(reader), new IeQualifierOfSetPointCommand(reader)}};
                    break;
                // 51
                case TypeId.C_BO_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeBinaryStateInformation(reader)}};
                    break;
                // 58
                case TypeId.C_SC_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeSingleCommand(reader), new IeTime56(reader)}};
                    break;
                // 59
                case TypeId.C_DC_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeDoubleCommand(reader), new IeTime56(reader)}};
                    break;
                // 60
                case TypeId.C_RC_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeBinaryStateInformation(reader), new IeTime56(reader)}};
                    break;
                // 61
                case TypeId.C_SE_TA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNormalizedValue(reader),
                            new IeQualifierOfSetPointCommand(reader), new IeTime56(reader)
                        }
                    };
                    break;
                // 62
                case TypeId.C_SE_TB_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeScaledValue(reader),
                            new IeQualifierOfSetPointCommand(reader), new IeTime56(reader)
                        }
                    };
                    break;
                // 63
                case TypeId.C_SE_TC_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeShortFloat(reader),
                            new IeQualifierOfSetPointCommand(reader), new IeTime56(reader)
                        }
                    };
                    break;
                // 64
                case TypeId.C_BO_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeBinaryStateInformation(reader), new IeTime56(reader)}};
                    break;
                // 70
                case TypeId.M_EI_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeCauseOfInitialization(reader)}};
                    break;
                // 100
                case TypeId.C_IC_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeQualifierOfInterrogation(reader)}};
                    break;
                // 101
                case TypeId.C_CI_NA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeQualifierOfCounterInterrogation(reader)}};
                    break;
                // 102
                case TypeId.C_RD_NA_1:
                    informationElements = new InformationElement[0][];
                    break;
                // 103
                case TypeId.C_CS_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeTime56(reader)}};
                    break;
                // 104
                case TypeId.C_TS_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeFixedTestBitPattern(reader)}};
                    break;
                // 105
                case TypeId.C_RP_NA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeQualifierOfResetProcessCommand(reader)}};
                    break;
                // 106
                case TypeId.C_CD_NA_1:
                    informationElements = new[] {new InformationElement[] {new IeTime16(reader)}};
                    break;
                // 107
                case TypeId.C_TS_TA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeTestSequenceCounter(reader), new IeTime56(reader)}};
                    break;
                // 110
                case TypeId.P_ME_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNormalizedValue(reader),
                            new IeQualifierOfParameterOfMeasuredValues(reader)
                        }
                    };
                    break;
                // 111
                case TypeId.P_ME_NB_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeScaledValue(reader),
                            new IeQualifierOfParameterOfMeasuredValues(reader)
                        }
                    };
                    break;
                // 112
                case TypeId.P_ME_NC_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeShortFloat(reader),
                            new IeQualifierOfParameterOfMeasuredValues(reader)
                        }
                    };
                    break;
                // 113
                case TypeId.P_AC_NA_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeQualifierOfParameterActivation(reader)}};
                    break;
                // 120
                case TypeId.F_FR_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNameOfFile(reader), new IeLengthOfFileOrSection(reader),
                            new IeFileReadyQualifier(reader)
                        }
                    };
                    break;
                // 121
                case TypeId.F_SR_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNameOfFile(reader), new IeNameOfSection(reader),
                            new IeLengthOfFileOrSection(reader), new IeSectionReadyQualifier(reader)
                        }
                    };
                    break;
                // 122
                case TypeId.F_SC_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNameOfFile(reader), new IeNameOfSection(reader),
                            new IeSelectAndCallQualifier(reader)
                        }
                    };
                    break;
                // 123
                case TypeId.F_LS_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNameOfFile(reader), new IeNameOfSection(reader),
                            new IeLastSectionOrSegmentQualifier(reader), new IeChecksum(reader)
                        }
                    };
                    break;
                // 124
                case TypeId.F_AF_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNameOfFile(reader), new IeNameOfSection(reader),
                            new IeAckFileOrSectionQualifier(reader)
                        }
                    };
                    break;
                // 125
                case TypeId.F_SG_NA_1:
                    informationElements = new[]
                    {
                        new InformationElement[]
                        {
                            new IeNameOfFile(reader), new IeNameOfSection(reader),
                            new IeFileSegment(reader)
                        }
                    };
                    break;
                // 126
                case TypeId.F_DR_TA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][];
                    for (var i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i] = new InformationElement[4];
                        informationElements[i][0] = new IeNameOfFile(reader);
                        informationElements[i][1] = new IeLengthOfFileOrSection(reader);
                        informationElements[i][2] = new IeStatusOfFile(reader);
                        informationElements[i][3] = new IeTime56(reader);
                    }
                    break;
                // 127
                case TypeId.F_SC_NB_1:
                    informationElements = new[]
                    {new InformationElement[] {new IeNameOfFile(reader), new IeTime56(reader), new IeTime56(reader)}};
                    break;
                default:
                    throw new IOException(
                        "Unable to parse Information Object because of unknown Type Identification: " + typeId);
            }
        }

        public int Encode(byte[] buffer, int i, ConnectionSettings settings)
        {
            var origi = i;

            buffer[i++] = (byte) informationObjectAddress;
            if (settings.IoaFieldLength > 1)
            {
                buffer[i++] = (byte) (informationObjectAddress >> 8);
                if (settings.IoaFieldLength > 2)
                {
                    buffer[i++] = (byte) (informationObjectAddress >> 16);
                }
            }

            i = informationElements.SelectMany(informationElementCombination => informationElementCombination)
                .Aggregate(i, (current, informationElement) => current + informationElement.Encode(buffer, current));

            return i - origi;
        }

        public int GetInformationObjectAddress()
        {
            return informationObjectAddress;
        }

        public InformationElement[][] GetInformationElements()
        {
            return informationElements;
        }

        public override string ToString()
        {
            var builder = new StringBuilder("IOA: " + informationObjectAddress);

            if (informationElements.Length > 1)
            {
                var i = 1;
                foreach (var informationElementSet in informationElements)
                {
                    builder.Append("\nInformation Element Set " + i + ":");
                    foreach (var informationElement in informationElementSet)
                    {
                        builder.Append("\n");
                        builder.Append(informationElement);
                    }
                    i++;
                }
            }
            else
            {
                foreach (var informationElementSet in informationElements)
                {
                    foreach (var informationElement in informationElementSet)
                    {
                        builder.Append("\n");
                        builder.Append(informationElement);
                    }
                }
            }

            return builder.ToString();
        }
    }
}