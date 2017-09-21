using System;
using System.Collections;
#if NETFX_CORE
using Windows.System.Threading;
#else
using System.Threading;
#endif

namespace GLTF
{
	public class AsyncAction
	{
		private bool _workerThreadRunning = false;
		private Exception _savedException;

		public IEnumerator RunOnWorkerThread(Action action)
		{
			_workerThreadRunning = true;

#if NETFX_CORE
			ThreadPool.RunAsync((_) =>
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

