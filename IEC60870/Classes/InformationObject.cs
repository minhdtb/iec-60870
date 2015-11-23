using IEC60870.IE;
using IEC60870.IE.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC60870.Classes
{
    public class InformationObject
    {
        private int informationObjectAddress;
        private InformationElement[][] informationElements;

        public InformationObject(int informationObjectAddress, InformationElement[][] informationElements)
        {
            this.informationObjectAddress = informationObjectAddress;
            this.informationElements = informationElements;
        }

        InformationObject(BinaryReader reader, TypeId typeId, int numberOfSequenceElements, ConnectionSettings settings)
        {
            switch (typeId)
            {
                // 1
                case TypeId.M_SP_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][1];
                    for (int i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i][0] = new IeSinglePointWithQuality(reader);
                    }
                    break;
                // 2
                case TypeId.M_SP_TA_1:
                    informationElements = new InformationElement[][] { { new IeSinglePointWithQuality(reader), new IeTime24(reader) } };
                    break;
                // 3
                case TypeId.M_DP_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][1];
                    for (int i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i][0] = new IeDoublePointWithQuality(is);
                    }
                    break;
                // 4
                case TypeId.M_DP_TA_1:
                    informationElements = new InformationElement[][] { { new IeDoublePointWithQuality(is), new IeTime24(is) } };
                    break;
                // 5
                case TypeId.M_ST_NA_1:
                    informationElements = new InformationElement[numberOfSequenceElements][2];
                    for (int i = 0; i < numberOfSequenceElements; i++)
                    {
                        informationElements[i][0] = new IeValueWithTransientState(is);
                        informationElements[i][1] = new IeQuality(is);
                    }
                    break;
            }
        }
    }
}
