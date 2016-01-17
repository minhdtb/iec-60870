using System.Threading;

namespace IEC60870.Util
{
    public abstract class ThreadBase
    {
        private readonly Thread thread;

        protected ThreadBase()
        {
            thread = new Thread(Run);
        }

        public bool IsAlive
        {
            get { return thread.IsAlive; }
        }

        public void Start()
        {
            thread.Start();
        }

        public void Join()
        {
            thread.Join();           
        }

        public void Abort()
        {
            thread.Abort();
        }

        public abstract void Run();
    }
}