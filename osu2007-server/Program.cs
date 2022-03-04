using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace osu2007server
{
	public class Program
	{

		private static HttpListener listener;
		public const string url = "http://localhost:8000/";
		private static int requestCount = 0; 

		public static async Task HandleIncomingConnections()
		{
			bool serverActive = true;

			while (serverActive)
			{
				// Awaiting user response here
				HttpListenerContext context = await listener.GetContextAsync();

				// Get the request/response objects
				HttpListenerRequest request = context.Request;
				HttpListenerResponse response = context.Response;

				// Print Request Info
				Console.WriteLine($"Request #: {++requestCount}");
				Console.WriteLine($"URL: {request.Url}");
				Console.WriteLine($"HTTP Method: {request.HttpMethod}");
				Console.WriteLine($"Host name: {request.UserHostName}");
				Console.WriteLine($"User Agent: {request.UserAgent}\n");

				// Write out response info
				byte[] data;

				switch (request.Url.AbsolutePath) {
					case "/web/osu-login.php": {
							string username = request.QueryString["username"];
							string password = request.QueryString["password"];
							data = Encoding.UTF8.GetBytes("1");
							break;
						}

					case "/web/osu-getscores.php": {
							string mapHash = request.QueryString["c"];
							data = Encoding.UTF8.GetBytes("0:Flux:500000:600:0:15:575:0:0:0:True:0\n0:Not Flux:420727:444:0:15:575:13213:0:0:True:4");
							break;
						}

					case "/web/osu-getuserinfo.php": {
							string username = request.QueryString["username"];
							data = Encoding.UTF8.GetBytes($"{username}|9|1111|-1|97.27|1");
							break;
						}

					case "/web/osu-submit.php": {
							string score_line = request.QueryString["score"];
							string password = request.QueryString["password"];

							int len = int.Parse(request.Headers["Content-Length"]);
							byte[] buffer = new byte[len];
							request.InputStream.Read(buffer, 0, len);

							string stringBuffer = Encoding.UTF8.GetString(buffer);
							List<byte> bytes = new List<byte>(buffer);
							string[] splitString = stringBuffer.Split('\n');
							int lengthOfFourLines = splitString[0].Length + splitString[1].Length +
							splitString[2].Length + splitString[3].Length + 4;
							bytes.RemoveRange(0, lengthOfFourLines);
							int lengthOfLastLine = splitString[^2].Length + 2;
							bytes.RemoveRange(bytes.Count - lengthOfLastLine, lengthOfLastLine);
							buffer = bytes.ToArray();

							File.WriteAllBytes("something.osr", buffer);

							data = new byte[0];
							break;
						}

					case "/web/osu-getreplay.php": {
							int id = int.Parse(request.QueryString["c"]);
							data = File.ReadAllBytes("something.osr");
							break;
                        }

					default: {
							response.StatusCode = 404;
							data = Encoding.UTF8.GetBytes("This works!");
							break;
					}
				}

				Console.WriteLine(request.Url.AbsolutePath);

				response.ContentType = "text/plain";
				response.ContentEncoding = Encoding.UTF8;
				response.ContentLength64 = data.LongLength;

				// Write to the response stream then close it
				response.OutputStream.Write(data, 0, data.Length);
				response.Close();

			}
		}

		public static void Main(string[] args)
		{
			Console.WriteLine("osu!2007 Server");

			// Create an HTTP Server; Start listening for a connection~
			listener = new HttpListener();
			listener.Prefixes.Add(url);
			listener.Start();
			Console.WriteLine($"Listening for connections on {url}");

			// Handle Requests
			Task listenTask = HandleIncomingConnections();
			listenTask.GetAwaiter().GetResult();


			// Close Listener
			listener.Close();
		}
	}
}
