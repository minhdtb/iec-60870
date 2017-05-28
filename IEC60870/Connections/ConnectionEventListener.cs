using System.IO;
using IEC60870.Object;

namespace IEC60870.Connections
{
    public class ConnectionEventListener
    {
        public delegate void ConnectionClosed(IOException e);

        public delegate void NewASdu(ASdu aSdu);
    }
}