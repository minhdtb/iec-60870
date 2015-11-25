using System;
using System.Threading;
using System.Threading.Tasks;

namespace IEC60870.Util
{
    public static class PeriodicTaskFactory
    {
        private static CancellationTokenSource source;
        public static void Cancel(this Task str)
        {
            source.Cancel();
        }

        public static Task Start(Action action, int delay)
        {
            source = new CancellationTokenSource();
            var token = source.Token;
            var task = Task.Delay(delay, token);
            task.ContinueWith((a) =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    action();
                }
                catch (Exception)
                {
                }
            });

            return task;
        }
    }
}
