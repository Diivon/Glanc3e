using System;
using System.Collections.Generic;
using System.Net;


namespace ConsoleApplication2
{
	class Program
	{
		static void Main(string[] args)
		{
			System.Threading.Tasks.Parallel.ForEach(getAllPicnames(), Ensure);
			Console.ReadKey();
		}
		static void Ensure(string a)
		{
			try
			{
				if(request(@"https://pp.userapi.com/c626122/v626122557/5aba1/" + a + ".jpg") == 200)
					Console.WriteLine(@"https://pp.userapi.com/c626122/v626122557/5aba1/" + a + ".jpg");
			}
			catch (Exception e)
			{}
		}
		static List<string> getSequence()
		{
			var result = new List<string>();
			for (var i = 65; i < 91; ++i)
				result.Add(((char)i).ToString());
			for (var i = 97; i < 123; ++i)
				result.Add(((char)i).ToString());
			return result;
		}
		static IEnumerable<string> getAllPicnames()
		{
			var result = new List<string>();
			foreach (var i0 in getSequence())
				foreach (var i1 in getSequence())
					foreach (var i2 in getSequence())
						foreach (var i3 in getSequence())
							foreach (var i4 in getSequence())
								foreach (var i5 in getSequence())
									foreach (var i6 in getSequence())
										foreach (var i7 in getSequence())
											foreach (var i8 in getSequence())
												foreach (var i9 in getSequence())
													foreach (var i10 in getSequence())
														yield return (i0 + i1 + i2 + i3 + i4 + i5 + i6 + i7 + i8 + i9 + i10);
		}
		static int request(string url)
		{
			int statusCode = 0;
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = WebRequestMethods.Http.Get;
				request.Accept = @"*/*";
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				if(response.ContentLength > 100)
					statusCode = (int)response.StatusCode;
				response.Close();
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
				statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
			}
			
			return statusCode;
		}
	}
}
