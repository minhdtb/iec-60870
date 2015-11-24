using IEC60870.Object;
using System.IO;

namespace IEC60870.Connection
{
    public class ConnectionEventListener
    {
        public delegate void newASdu(ASdu aSdu);

        public delegate void connectionClosed(IOException e);
    }
}
