using System.IO;
using IEC60870.IE.Base;

namespace IEC60870.IE
{
    public class IeStatusOfFile : InformationElement
    {
        private readonly bool lastFileOfDirectory;
        private readonly bool nameDefinesDirectory;
        private readonly int status;
        private readonly bool transferIsActive;

        public IeStatusOfFile(int status, bool lastFileOfDirectory, bool nameDefinesDirectory,
            bool transferIsActive)
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
            lastFileOfDirectory = (b1 & 0x20) == 0x20;
            nameDefinesDirectory = (b1 & 0x40) == 0x40;
            transferIsActive = (b1 & 0x80) == 0x80;
        }

        public override int Encode(byte[] buffer, int i)
        {
            buffer[i] = (byte) status;
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

        public int GetStatus()
        {
            return status;
        }

        public bool IsLastFileOfDirectory()
        {
            return lastFileOfDirectory;
        }

        public bool IsNameDefinesDirectory()
        {
            return nameDefinesDirectory;
        }

        public bool IsTransferIsActive()
        {
            return transferIsActive;
        }

        public override string ToString()
        {
            return "Status of file: " + status + ", last file of directory: " + lastFileOfDirectory
                   + ", name defines directory: " + nameDefinesDirectory + ", transfer is active: " + transferIsActive;
        }
    }
}