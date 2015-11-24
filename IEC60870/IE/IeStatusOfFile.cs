using IEC60870.IE.Base;
using System;
using System.IO;

namespace IEC60870.IE
{
    public class IeStatusOfFile : InformationElement
    {
        private int status;
        private Boolean lastFileOfDirectory;
        private Boolean nameDefinesDirectory;
        private Boolean transferIsActive;

        public IeStatusOfFile(int status, Boolean lastFileOfDirectory, Boolean nameDefinesDirectory,
                Boolean transferIsActive)
        {
            this.status = status;
            this.lastFileOfDirectory = lastFileOfDirectory;
            this.nameDefinesDirectory = nameDefinesDirectory;
            this.transferIsActive = transferIsActive;
        }

        public IeStatusOfFile(BinaryReader reader)
        {
            int b1 = reader.ReadByte();
            status = b1 & 0x1f;
            lastFileOfDirectory = ((b1 & 0x20) == 0x20);
            nameDefinesDirectory = ((b1 & 0x40) == 0x40);
            transferIsActive = ((b1 & 0x80) == 0x80);
        }

        public override int encode(byte[] buffer, int i)
        {
            buffer[i] = (byte)status;
            if (lastFileOfDirectory)
            {
                buffer[i] |= 0x20;
            }
            if (nameDefinesDirectory)
            {
                buffer[i] |= 0x40;
            }
            if (transferIsActive)
            {
                buffer[i] |= 0x80;
            }
            return 1;
        }

        public int getStatus()
        {
            return status;
        }

        public Boolean isLastFileOfDirectory()
        {
            return lastFileOfDirectory;
        }

        public Boolean isNameDefinesDirectory()
        {
            return nameDefinesDirectory;
        }

        public Boolean isTransferIsActive()
        {
            return transferIsActive;
        }

        public override String ToString()
        {
            return "Status of file: " + status + ", last file of directory: " + lastFileOfDirectory
                    + ", name defines directory: " + nameDefinesDirectory + ", transfer is active: " + transferIsActive;
        }
    }
}
