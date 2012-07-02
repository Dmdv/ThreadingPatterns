using System;
using System.Diagnostics;
using ThreadingPatterns.FixedThreadPool;

namespace UnitTests
{
	public class TestLog : ILog
	{
		public void Info(string msg)
		{
			Trace.WriteLine(msg);
		}

		public void Exception(Exception ex)
		{
			Trace.WriteLine(ex.ToString());
		}
	}
}