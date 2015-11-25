using System;
using System.Threading;
using System.Threading.Tasks;

namespace IEC60870.Util
{
    public static class PeriodicTaskFactory
    {        
        public static CancellationTokenSource Start(Action action, int delay)
        {
            var source = new CancellationTokenSource();
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

            return source;
        }
    }
}
