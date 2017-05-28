using System;
using System.Threading;
using System.Threading.Tasks;

namespace IEC60870.Utils
{
    public class RunTask
    {
        private readonly CancellationTokenSource source;

        public RunTask()
        {
            source = new CancellationTokenSource();
        }

        public CancellationToken GetToken()
        {
            return source.Token;
        }

        public void Cancel()
        {
            source.Cancel();
        }
    }

    public class PeriodicTaskFactory
    {
        public static RunTask Start(Action action, int delay)
        {
            var run = new RunTask();
            var task = Task.Delay(delay, run.GetToken());
            task.ContinueWith(t =>
            {
                try
                {
                    run.GetToken().ThrowIfCancellationRequested();
                    action();
                }
                catch (Exception)
                {
                    // ignored
                }
            });

            return run;
        }
    }
}