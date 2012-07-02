using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadingPatterns.FixedThreadPool;

namespace UnitTests
{
	/// <summary>
	/// Summary description for FixedThreadPooltests
	/// </summary>
	[TestClass]
	public class FixedThreadPooltests
	{
		private static TestTask[] _tasks;
		private static TestTask[] _lowtasks;
		private static TestTask[] _normallowtasks;
		private static TestTask[] _highlowtasks;

		/// <summary>
		/// Integration test. Ensure all tasks are completed and no exception.
		/// </summary>
		[TestMethod]
		public void TestThreads()
		{
			var pool = new FixedThreadPool(10)
			{
				Log = new TestLog()
			};
			try
			{
				pool.Execute(new TestTask(Priority.High, 500)
				{
					Name = "Task 1"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.Normal, 1000)
				{
					Name = "Task 2"
				}, Priority.Normal);
				pool.Execute(new TestTask(Priority.Low, 1500)
				{
					Name = "Task 3"
				}, Priority.Low);
				pool.Execute(new TestTask(Priority.High, 100)
				{
					Name = "Task 4"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.High, 1500)
				{
					Name = "Task 5"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.Low, 2000)
				{
					Name = "Task 6"
				}, Priority.Low);
				pool.Execute(new TestTask(Priority.High, 100)
				{
					Name = "Task 7"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.High, 1500)
				{
					Name = "Task 8"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.Low, 2000)
				{
					Name = "Task 9"
				}, Priority.Low);
				pool.Execute(new TestTask(Priority.Normal, 2000)
				{
					Name = "Task 10"
				}, Priority.Normal);
				pool.Join();
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.ToString());
			}
		}

		[TestMethod]
		public void TestThreadsOrders()
		{
			var pool = new FixedThreadPool(10)
			{
				Log = new TestLog()
			};
			try
			{
				pool.Execute(new TestTask(Priority.High, 500)
				{
					Name = "Task 1"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.Normal, 500)
				{
					Name = "Task 2"
				}, Priority.Normal);
				pool.Execute(new TestTask(Priority.Low, 500)
				{
					Name = "Task 3"
				}, Priority.Low);
				pool.Execute(new TestTask(Priority.High, 500)
				{
					Name = "Task 4"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.High, 500)
				{
					Name = "Task 5"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.Low, 500)
				{
					Name = "Task 6"
				}, Priority.Low);
				pool.Execute(new TestTask(Priority.High, 500)
				{
					Name = "Task 7"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.High, 500)
				{
					Name = "Task 8"
				}, Priority.High);
				pool.Execute(new TestTask(Priority.Low, 500)
				{
					Name = "Task 9"
				}, Priority.Low);
				pool.Execute(new TestTask(Priority.Normal, 500)
				{
					Name = "Task 10"
				}, Priority.Normal);
				pool.Join();
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.ToString());
			}
		}

		[TestMethod]
		public void AssumeTaskOrderCorrect()
		{
			ITask task;

			var queue = new TaskPriorityQueue();

			foreach (var testTask in _tasks)
			{
				queue.Enqueue(testTask, testTask.Priority);
			}

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.High);

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.High);

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Normal);

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Low);

			Assert.IsFalse(queue.Dequeue(out task));

			foreach (var testTask in _lowtasks)
			{
				queue.Enqueue(testTask, testTask.Priority);
			}

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Low);

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Low);

			Assert.IsFalse(queue.Dequeue(out task));

			foreach (var testTask in _normallowtasks)
			{
				queue.Enqueue(testTask, testTask.Priority);
			}

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Normal);

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Low);

			Assert.IsFalse(queue.Dequeue(out task));

			foreach (var testTask in _highlowtasks)
			{
				queue.Enqueue(testTask, testTask.Priority);
			}

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.High);

			Assert.IsTrue(queue.Dequeue(out task));
			Assert.IsTrue(((TestTask) task).Priority == Priority.Low);

			Assert.IsFalse(queue.Dequeue(out task));
		}

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			_tasks = new[]
			{
				new TestTask(Priority.High),
				new TestTask(Priority.High),
				new TestTask(Priority.Normal),
				new TestTask(Priority.Low)
			};

			_lowtasks = new[]
			{
				new TestTask(Priority.Low),
				new TestTask(Priority.Low)
			};

			_normallowtasks = new[]
			{
				new TestTask(Priority.Low),
				new TestTask(Priority.Normal)
			};

			_highlowtasks = new[]
			{
				new TestTask(Priority.Low),
				new TestTask(Priority.High)
			};
		}
	}
}