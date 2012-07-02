using System;
using System.Collections.Concurrent;

namespace ThreadingPatterns.FixedThreadPool
{
	/// <summary>
	/// ќчередь задач с приоритетами. 
	/// ѕоскольку приоритетов - небольшое число, нет необходимости создавать классическую очередь с приоритетами.
	/// </summary>
	public class TaskPriorityQueue
	{
		private readonly ConcurrentQueue<ITask> _high = new ConcurrentQueue<ITask>();
		private readonly ConcurrentQueue<ITask> _low = new ConcurrentQueue<ITask>();
		private readonly ConcurrentQueue<ITask> _normal = new ConcurrentQueue<ITask>();

		private readonly object _sync = new object();
		private int _highDequeued;

		public void Enqueue(ITask task, Priority priority)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}

			switch (priority)
			{
				case Priority.High:
					_high.Enqueue(task);
					break;
				case Priority.Normal:
					_normal.Enqueue(task);
					break;
				default:
					_low.Enqueue(task);
					break;
			}
		}

		/// <summary>
		/// Returns null if there's no task in the queue.
		/// </summary>
		/// <returns></returns>
		public bool Dequeue(out ITask task)
		{
			lock (_sync)
			{
				if (_high.Count == 0)
				{
					_highDequeued = 0;
					return _normal.Count == 0 ? _low.TryDequeue(out task) : _normal.TryDequeue(out task);
				}

				if (_highDequeued < 2 && _high.TryDequeue(out task))
				{
					_highDequeued++;
					return true;
				}

				if (_highDequeued == 2 && _normal.TryDequeue(out task))
				{
					_highDequeued = 0;
					return true;
				}
			}

			task = null;
			return false;
		}
	}
}