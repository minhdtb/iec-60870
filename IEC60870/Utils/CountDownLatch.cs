using System.Threading;

namespace IEC60870.Utils
{
    public class CountDownLatch
    {
        private readonly EventWaitHandle _event;
        private int _remain;

        public CountDownLatch(int count)
        {
            _remain = count;
            _event = new ManualResetEvent(false);
        }

        public void CountDown()
        {
            if (Interlocked.Decrement(ref _remain) == 0)
                _event.Set();
        }

        public void Wait(int timeout)
        {
            _event.WaitOne(timeout);
        }

        public void Wait()
        {
            _event.WaitOne();
        }
    }
}