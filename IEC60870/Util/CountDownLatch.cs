using System.Threading;

namespace IEC60870.Util
{
    public class CountDownLatch
    {
        private int remain;
        private EventWaitHandle @event;

        public CountDownLatch(int count)
        {
            remain = count;
            @event = new ManualResetEvent(false);
        }

        public void Signal()
        {            
            if (Interlocked.Decrement(ref remain) == 0)
                @event.Set();
        }

        public void Wait(int timeout)
        {
            @event.WaitOne(timeout);            
        }

        public void Wait()
        {
            @event.WaitOne();
        }
    }
}
