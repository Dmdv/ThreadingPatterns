namespace ThreadingPatterns.FixedThreadPool
{
	public interface IThreadPool
	{
		bool Execute(ITask task, Priority priority);
		void Join();
	}
}