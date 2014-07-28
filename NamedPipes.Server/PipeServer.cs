namespace NamedPipes.Server
{
	using System;
	using System.IO.Pipes;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	public class PipeServer
	{
		private readonly CancellationToken _cancel;

		private readonly CancellationTokenSource _cancelSource;

		private readonly string _pipeName;

		public PipeServer(string pipeName)
		{
			_pipeName = pipeName;

			_cancelSource = new CancellationTokenSource();

			_cancel = _cancelSource.Token;
		}

		public async Task Start()
		{
			Console.WriteLine("[thread: {0}] -> Starting server listener.", Thread.CurrentThread.ManagedThreadId);

			while (!_cancel.IsCancellationRequested)
			{
				await Listener();
			}
		}

		public void Stop()
		{
			_cancelSource.Cancel();
		}

		private async Task Listener()
		{
			using (NamedPipeServerStream server = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
			{
				Console.WriteLine("\r\n[thread: {0}] -> Waiting for client.", Thread.CurrentThread.ManagedThreadId);

				await Task.Factory.FromAsync(server.BeginWaitForConnection, server.EndWaitForConnection, null);

				Console.WriteLine("[thread: {0}] -> Client connected.", Thread.CurrentThread.ManagedThreadId);

				await ReadData(server);

				if (server.IsConnected)
				{
					server.Disconnect();
				}
			}
		}

		private async Task ReadData(NamedPipeServerStream server)
		{
			Console.WriteLine("[thread: {0}] -> Reading data.", Thread.CurrentThread.ManagedThreadId);

			byte[] buffer = new byte[255];

			int length = await server.ReadAsync(buffer, 0, buffer.Length, _cancel);

			byte[] chunk = new byte[length];

			Array.Copy(buffer, chunk, length);

			string content = Encoding.UTF8.GetString(chunk);

			Console.WriteLine("[thread: {0}] -> {1}: {2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, content);

			if (content == "exit")
			{
				Stop();
			}
		}
	}
}