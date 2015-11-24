using System.Threading;

namespace IEC60870.Util
{
    public abstract class ThreadBase
    {
        private Thread thread;

        protected ThreadBase() { thread = new Thread(new ThreadStart(this.Run)); }

        public void Start() { thread.Start(); }
        public void Join() { thread.Join(); }
        public bool IsAlive { get { return thread.IsAlive; } }

        public abstract void Run();
    }
}
