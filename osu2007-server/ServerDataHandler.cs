using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace osu2007server
{
    public static class ServerDataHandler
	{
		// Configuration for the server
		public static string server = "localhost";
		public static string database = "osu2007";
		public static string userid;
		private static string password;
		private static string connString;
		private static MySqlConnection connection;
		private static SHA512 shaCalculator;

		public static void SetUser(string u, string p)
		{
			userid = u;
			password = p;
			connString = $@"server={server};userid={userid};password={password};database={database}";
			connection = new MySqlConnection(connString);
			// Here to test connection to SQL server
			try
            {
				connection.Open();
			} catch (MySqlException)
            {
				Console.WriteLine("ERROR: Invalid Password. Closing application.");
				Environment.Exit(401);
            }
			
			connection.Close();
		}

		public static byte[] Login(HttpListenerRequest r)
		{
			string username = r.QueryString["username"];
			string password = r.QueryString["password"];

			if (username.Length >= 3 && IsValidHash(password) && !username.Contains(':'))
            {
				// Start SQL Code
				connection.Open();

				

				string sql = $"SELECT * from `users` WHERE `username` = '{username}'";
				using var cmd = new MySqlCommand(sql, connection);

				using MySqlDataReader rdr = cmd.ExecuteReader();
				

				if (!rdr.HasRows)
				{
					rdr.Close();
					Console.WriteLine($"User {username} not found. creating account...");
					sql = "INSERT INTO `users` VALUES(NULL, @username, @password, 0, 0, 0, 0)";
					using var cmd2 = new MySqlCommand(sql, connection);

					cmd2.Parameters.AddWithValue("@username", username);
					cmd2.Parameters.AddWithValue("@password", password);
					cmd2.Prepare();

					cmd2.ExecuteNonQuery();

					Console.WriteLine($"Successfully created account {username}.");
					connection.Close();

					string a = "1";
					return Encoding.UTF8.GetBytes(a); 
					
				} else
                {
					Console.WriteLine($"User {username} found. Attempting to Authenticate...");

					rdr.Read();

					if (password == rdr.GetString("password"))
                    {
						rdr.Close();
						connection.Close();
						
						Console.WriteLine($"Authentication Succeeded. Logged in as {username}.");
						return Encoding.UTF8.GetBytes("1");
					} else
                    {
						rdr.Close();
						connection.Close();
						Console.WriteLine($"Authentication for {username} failed.");
						return Encoding.UTF8.GetBytes("0");
					}


				}

				// End SQL Code
			} else
            {
				connection.Close();
				return Encoding.UTF8.GetBytes("0");
			}


			
		}
		
		public static byte[] GetScores(HttpListenerRequest r)
		{
			string mapHash = r.QueryString["c"];
			return Encoding.UTF8.GetBytes("0:Flux:500000:600:0:15:575:0:0:0:True:0\n0:Not Flux:420727:444:0:15:575:13213:0:0:True:4");
		}

		public static byte[] GetUserInfo(HttpListenerRequest r)
		{
			string username = r.QueryString["username"];
			return Encoding.UTF8.GetBytes($"{username}|9|1111|-1|97.27|1");
		}

		public static byte[] SubmitScore(HttpListenerRequest r)
		{
			string score_line = r.QueryString["score"];
			string password = r.QueryString["password"];

			int len = int.Parse(r.Headers["Content-Length"]);
			byte[] buffer = new byte[len];
			r.InputStream.Read(buffer, 0, len);

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

			return new byte[0];
		}

		public static byte[] GetReplay(HttpListenerRequest r)
		{
			int id = int.Parse(r.QueryString["c"]);
			return File.ReadAllBytes("something.osr");
		}

		public static bool IsValidHash(string input)
		{
			if (String.IsNullOrEmpty(input))
			{
				return false;
			}

			return Regex.IsMatch(input, "^[0-9a-fA-F]{128}$", RegexOptions.Compiled);
		}

	}
}
