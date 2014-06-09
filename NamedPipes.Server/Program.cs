namespace NamedPipes.Server
{
	using System;
	using System.Threading;

	internal class Program
	{
		private static readonly PipeServer _pipeServer;
		
		static Program()
		{
			_pipeServer = new PipeServer("7988717B-3119-4413-8636-E6CE325E3957");
		}

		private static void Main(string[] args)
		{
			Console.WriteLine("[thread: {0}] -> Starting server.", Thread.CurrentThread.ManagedThreadId);

			_pipeServer.Start().Wait();

			Console.WriteLine("\r\n[thread: {0}] -> Server closed.\r\n", Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine("Press any key to continue ...");
			Console.ReadKey();
		}
	}
}