using System;

namespace ThreadingPatterns.FixedThreadPool
{
	public interface ILog
	{
		void Info(string msg);
		void Exception(Exception ex);
	}
}