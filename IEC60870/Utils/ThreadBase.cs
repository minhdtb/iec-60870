using System.Threading;

namespace IEC60870.Utils
{
    public abstract class ThreadBase
    {
        private readonly Thread thread;

        protected ThreadBase()
        {
            thread = new Thread(Run);
        }

        public bool IsAlive => thread.IsAlive;

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