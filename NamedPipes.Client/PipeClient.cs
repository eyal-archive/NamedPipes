namespace NamedPipes.Client
{
	using System;
	using System.Diagnostics;
	using System.IO.Pipes;
	using System.Text;
	using System.Threading.Tasks;

	public class PipeClient
	{
		private readonly string _pipeName;

		public PipeClient(string pipeName)
		{
			_pipeName = pipeName;
		}

		public async Task Send(string content, int timeOut = 30000)
		{
			try
			{
				using (NamedPipeClientStream client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out, PipeOptions.Asynchronous))
				{
					client.Connect(timeOut);

					if (client.IsConnected)
					{
						byte[] output = Encoding.UTF8.GetBytes(content);

						await client.WriteAsync(output, 0, output.Length).ConfigureAwait(false);

						await client.FlushAsync();
					}
				}
			}
			catch (TimeoutException ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}