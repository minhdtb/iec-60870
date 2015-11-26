using System.Threading;

namespace IEC60870.Util
{
    public class CountDownLatch
    {
        private readonly EventWaitHandle @event;
        private int remain;

        public CountDownLatch(int count)
        {
            remain = count;
            @event = new ManualResetEvent(false);
        }

        public void CountDown()
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