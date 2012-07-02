using System;

namespace ThreadingPatterns.FixedThreadPool
{
	internal class NullLog : ILog
	{
		public void Info(string msg)
		{
		}

		public void Exception(Exception ex)
		{
		}
	}
}