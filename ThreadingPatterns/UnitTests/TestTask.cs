using System.Threading;
using ThreadingPatterns.FixedThreadPool;

namespace UnitTests
{
	public class TestTask : ITask
	{
		private readonly int _ms;

		public TestTask(Priority priority, int ms = 0)
		{
			_ms = ms;
			Priority = priority;
		}

		public Priority Priority { get; private set; }

		public string Name { get; set; }

		public void Excecute()
		{
			Thread.Sleep(_ms);
		}

		public override string ToString()
		{
			return string.Format("Name = {0}, Priority = {1}", Name, Priority);
		}
	}
}