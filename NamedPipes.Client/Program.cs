namespace NamedPipes.Client
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	internal class Program
	{
		private static readonly CancellationToken _cancel;

		private static readonly CancellationTokenSource _cancelSource;

		private static readonly PipeClient _pipeClient;

		static Program()
		{
			_pipeClient = new PipeClient("7988717B-3119-4413-8636-E6CE325E3957");

			_cancelSource = new CancellationTokenSource();

			_cancel = _cancelSource.Token;
		}

		private static void Main(string[] args)
		{
			Console.WriteLine("[thread: {0}] -> Starting client session. ", Thread.CurrentThread.ManagedThreadId);

			Task.Run(async () =>
			               {
				               StringBuilder input = new StringBuilder();

							   Console.WriteLine("[thread: {0}] -> waiting for server.", Thread.CurrentThread.ManagedThreadId);

				               while (!_cancelSource.IsCancellationRequested)
				               {
					               ConsoleKeyInfo key = Console.ReadKey();

					               if (key.Key == ConsoleKey.Enter)
					               {
						               string content = input.ToString().TrimEnd();

						               if (!string.IsNullOrWhiteSpace(content))
						               {
										   await _pipeClient.Send(content);
						               }

									   if (content.ToLower() == "exit")
									   {
										   _cancelSource.Cancel();
									   }

						               input.Clear();

						               Console.WriteLine();
					               }
					               else if (key.Key == ConsoleKey.Backspace)
					               {
						               if (input.Length > 0)
						               {
							               input = input.Remove(input.Length - 1, 1);
							               Console.Write(" \b"); 
						               }
					               }
					               else
					               {
						               input.Append(key.KeyChar);
					               }
				               }
			               }, _cancel).Wait();

			Console.WriteLine("\r\n[thread: {0}] -> Client session was ended.\r\n", Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine("Press any key to continue ...");
			Console.ReadKey();
		}
	}
}