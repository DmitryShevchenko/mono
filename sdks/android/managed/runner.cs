using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Mono
{
	public class TestRunner
	{
		[DllImport("__Internal")]
		static extern void simple_log(string message);

		public static int Main() {
			simple_log("Test");
//			Environment.SetEnvironmentVariable("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "1");
			simple_log(String.Format("Format: " + 1.12));
			simple_log("CurrentCulture: " + CultureInfo.CurrentCulture);
			simple_log("DateTime.Now: " + DateTime.Now);
			simple_log("RuntimeInfo: " + RuntimeInformation.FrameworkDescription);
			simple_log("OS: " + RuntimeInformation.OSDescription);

			var message = new StringBuilder();
			message.AppendLine("GET / HTTP/1.1");
			message.AppendLine("Host: www.example.com");
			message.AppendLine("Connection: close");
			message.AppendLine();

			var client = new TcpClient("www.example.com", 80);
			client.SendTimeout = 500;
			client.ReceiveTimeout = 1000;
			var data = Encoding.ASCII.GetBytes(message.ToString());

			var stream = client.GetStream();

			stream.Write(data, 0, data.Length);

			// receive data
			using (var memory = new MemoryStream())
			{
				stream.CopyTo(memory);
				memory.Position = 0;
				var responseData = memory.ToArray();
				var response = Encoding.ASCII.GetString(responseData, 0, responseData.Length);
				simple_log(response);
			}

			stream.Close();
			client.Close();

			return 17;
		}
	}
}