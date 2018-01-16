using System;
using System.Collections;
#if WINDOWS_UWP
using System.Threading.Tasks;
#else
using System.Threading;
#endif

namespace UnityGLTF
{
    /// <summary>
    /// Creates a thread to run multithreaded operations on
    /// </summary>
    public class AsyncAction
    {
        private bool _workerThreadRunning = false;
        private Exception _savedException;

        public IEnumerator RunOnWorkerThread(Action action)
        {
            _workerThreadRunning = true;

#if WINDOWS_UWP
            Task.Factory.StartNew(() =>
#else
            ThreadPool.QueueUserWorkItem((_) =>
#endif
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    _savedException = e;
                }

                _workerThreadRunning = false;
            });

            yield return Wait();

            if (_savedException != null)
            {
                throw _savedException;
            }
        }

        private IEnumerator Wait()
        {
            while (_workerThreadRunning)
            {
                yield return null;
            }
        }
    }
}
