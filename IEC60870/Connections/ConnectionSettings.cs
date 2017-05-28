namespace IEC60870.Connections
{
    public class ConnectionSettings
    {
        public int CommonAddressFieldLength = 2;

        public int CotFieldLength = 2;
        public int IoaFieldLength = 3;
        public int MaxIdleTime = 20000;

        public int MaxTimeNoAckReceived = 15000;
        public int MaxTimeNoAckSent = 10000;

        public int MaxUnconfirmedIPdusReceived = 8;
        public int MessageFragmentTimeout = 5000;

        public ConnectionSettings GetCopy()
        {
            var settings = new ConnectionSettings();

            settings.MessageFragmentTimeout = MessageFragmentTimeout;

            settings.CotFieldLength = CotFieldLength;
            settings.CommonAddressFieldLength = CommonAddressFieldLength;
            settings.IoaFieldLength = IoaFieldLength;

            settings.MaxTimeNoAckReceived = MaxTimeNoAckReceived;
            settings.MaxTimeNoAckSent = MaxTimeNoAckSent;
            settings.MaxIdleTime = MaxIdleTime;

            settings.MaxUnconfirmedIPdusReceived = MaxUnconfirmedIPdusReceived;

            return settings;
        }
    }
}