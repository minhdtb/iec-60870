namespace IEC60870.Classes
{
    public class ConnectionSettings
    {
        public int messageFragmentTimeout = 5000;

        public int cotFieldLength = 2;
        public int commonAddressFieldLength = 2;
        public int ioaFieldLength = 3;

        public int maxTimeNoAckReceived = 15000;
        public int maxTimeNoAckSent = 10000;
        public int maxIdleTime = 20000;

        public int maxUnconfirmedIPdusReceived = 8;

        public ConnectionSettings getCopy()
        {
            ConnectionSettings settings = new ConnectionSettings();

            settings.messageFragmentTimeout = messageFragmentTimeout;

            settings.cotFieldLength = cotFieldLength;
            settings.commonAddressFieldLength = commonAddressFieldLength;
            settings.ioaFieldLength = ioaFieldLength;

            settings.maxTimeNoAckReceived = maxTimeNoAckReceived;
            settings.maxTimeNoAckSent = maxTimeNoAckSent;
            settings.maxIdleTime = maxIdleTime;

            settings.maxUnconfirmedIPdusReceived = maxUnconfirmedIPdusReceived;

            return settings;
        }
    }
}
