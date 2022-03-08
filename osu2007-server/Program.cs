using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace osu2007server
{
	public static class Program
	{

		private static HttpListener listener;
		public const string url = "http://localhost:8000/";
		private static int requestCount = 0; 

		public static void HandleIncomingConnections()
		{
			bool serverActive = true;

			while (serverActive)
			{
				// Awaiting user response here
				HttpListenerContext context = listener.GetContext();

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
							ServerDataHandler.Login(request, response);
							break;
						}

					case "/web/osu-getscores.php": {
							data = ServerDataHandler.GetScores(request);
							break;
						}

					case "/web/osu-getuserinfo.php": {
							data = ServerDataHandler.GetUserInfo(request);
							break;
						}

					case "/web/osu-submit.php": {
							data = ServerDataHandler.SubmitScore(request);
							break;
						}

					case "/web/osu-getreplay.php": {
							data = ServerDataHandler.GetReplay(request);
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

				// Write to the response stream then close it
				response.Close();

			}
		}

		public static void Main(string[] args)
		{
			Console.WriteLine("osu!2007 Server");

			// Accept input for Database login
			// (This is crappy code I am sorry
			// to anyone who has had to read
			// this bullshit code)
			Console.WriteLine($"Target Server: {ServerDataHandler.server}");
			Console.WriteLine($"Target Database: {ServerDataHandler.database}");
			Console.Write("Database Username: ");
			string username = Console.ReadLine();
			Console.Write("Database Password: ");
			string password = string.Empty;
			ConsoleKey key;
			do
			{
				var keyInfo = Console.ReadKey(intercept: true);
				key = keyInfo.Key;

				if (key == ConsoleKey.Backspace && password.Length > 0)
				{
					Console.Write("\b \b");
					password = password[0..^1];
				}
				else if (!char.IsControl(keyInfo.KeyChar))
				{
					Console.Write("*");
					password += keyInfo.KeyChar;
				}
			} while (key != ConsoleKey.Enter);
			Console.WriteLine();

			// Set the SQL Database user
			ServerDataHandler.SetUser(username, password);

			// Create an HTTP Server; Start listening for a connection~
			listener = new HttpListener();
			listener.Prefixes.Add(url);
			listener.Start();
			Console.WriteLine($"Listening for connections on {url}");

			// Handle Requests
			HandleIncomingConnections();
			listenTask.GetAwaiter().GetResult();


			// Close Listener
			listener.Close();
		}
	}
}
