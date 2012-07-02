using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadingPatterns;

namespace UnitTests
{
	[TestClass]
	public class BlockingQueueTests
	{
		[TestMethod]
		public void Test()
		{
			var q = new BlockingQueue<int>(4);

			// Producer
			new Thread(() =>
			{
				for (var x = 0;; x++)
				{
					if (!q.Enqueue(x))
						break;
					Trace.WriteLine(x.ToString("0000") + " >");
				}
				Trace.WriteLine("Producer quitting");
			}).Start();

			// Consumers
			for (var i = 0; i < 2; i++)
			{
				new Thread(() =>
				{
					for (;;)
					{
						Thread.Sleep(100);
						int x;
						if (!q.Dequeue(out x))
							break;
						Trace.WriteLine("     < " + x.ToString("0000"));
					}
					Trace.WriteLine("Consumer quitting");
				}).Start();
			}

			Thread.Sleep(2000);

			Trace.WriteLine("Quitting");

			q.Quit();
		}
	}
}