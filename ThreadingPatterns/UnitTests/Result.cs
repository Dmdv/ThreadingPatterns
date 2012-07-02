namespace UnitTests
{
	public struct Result
	{
		public Result(double free, double all)
		{
			Free = free;
			All = all;
		}

		public readonly double All;
		public readonly double Free;
	}
}