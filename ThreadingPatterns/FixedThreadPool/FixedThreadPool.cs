using System;
using System.Collections;
using System.Threading;

namespace ThreadingPatterns.FixedThreadPool
{
	/// <summary>
	/// This class is responsible only for internal threads safety.
	/// To enhance thread safety of FixedThreadPool in several threads, use external locks.
	/// </summary>
	public class FixedThreadPool : IThreadPool
	{
		private readonly TaskPriorityQueue _queue = new TaskPriorityQueue();
		private readonly ArrayList _threadList = ArrayList.Synchronized(new ArrayList());
		private readonly ManualResetEvent _wakeup = new ManualResetEvent(false);
		private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
		private bool _continueRun;

		public FixedThreadPool(int count)
		{
			_continueRun = true;

			for (var i = 0; i < count; i++)
			{
				var t = new Thread(ThreadFunc) { Name = "Worker" + i };
				t.Start();
				_threadList.Add(t);
			}
		}

		public ILog Log { get; set; }

		public bool Execute(ITask task, Priority priority)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}

			if (!_continueRun)
				return false;

			_queue.Enqueue(task, priority);
			_wakeup.Set(); // wake up thread to process this task
			_wakeup.Reset();
			return true;
		}

		/// <summary>
		/// Shutdown each thread
		/// </summary>
		public void Join()
		{
			_continueRun = false;
			_wakeup.Set(); // wake up all the thread
			_stopEvent.WaitOne();
		}

		private void ThreadFunc()
		{
			try
			{
				while (_continueRun)
				{
					ITask task;

					while (_queue.Dequeue(out task))
					{
						task.Excecute();
						Log.Info(string.Format("Finished : {0}", task));
					}

					_wakeup.WaitOne();
				}
			}
			catch(ThreadAbortException ex)
			{
				Log.Exception(ex);
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
			finally
			{
				_threadList.Remove(Thread.CurrentThread);
				if (_threadList.Count == 0)
				{
					_stopEvent.Set();
				}
			}
		}
	}
}