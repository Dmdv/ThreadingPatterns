using System.Collections.Generic;
using System.Threading;

namespace ThreadingPatterns
{
	/// <summary>
	/// http://www.codeproject.com/Articles/28785/Thread-synchronization-Wait-and-Pulse-demystified
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BlockingQueue<T>
	{
		private readonly object _key = new object();
		private readonly Queue<T> _queue = new Queue<T>();
		private readonly int _size;
		private bool _quit;

		public BlockingQueue(int size)
		{
			_size = size;
		}

		public void Quit()
		{
			lock (_key)
			{
				_quit = true;

				Monitor.PulseAll(_key);
			}
		}

		public bool Enqueue(T t)
		{
			lock (_key)
			{
				while (!_quit && _queue.Count >= _size)
				{
					Monitor.Wait(_key);
				}

				if (_quit)
					return false;

				_queue.Enqueue(t);

				Monitor.PulseAll(_key);
			}

			return true;
		}

		public bool Dequeue(out T t)
		{
			t = default(T);

			lock (_key)
			{
				while (!_quit && _queue.Count == 0)
				{
					Monitor.Wait(_key);
				}

				if (_queue.Count == 0)
					return false;

				t = _queue.Dequeue();

				Monitor.PulseAll(_key);
			}

			return true;
		}
	}
}