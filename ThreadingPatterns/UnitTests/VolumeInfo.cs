using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace UnitTests
{
	/// <summary>
	/// Usage:
	/// 	var info = new VolumeInfo();
	///		var volumes = info.GetAvailableBytes();
	///		var all = volumes.All;
	/// </summary>
	public class VolumeInfo
	{
		private const string Address = "http://kldfs.avp.ru/status";

		private const string Pattern =
			@"(?:<\s*li[^>]*>){1}(?:space\s*usage:\s*free\s*)([\d]+\.[\d]+)\s*([PpTtMmKk]{1}[Bb]{1})\s*of\s*([\d]+\.[\d]+)\s*([PpTtMmKk]{1}[Bb]{1})\s*(?:[\d\.,\(\)\s*%\s*]*)(?:<\s*/li\s*>){1}";

		private readonly Regex _regex;

		public VolumeInfo()
		{
			_regex = new Regex(Pattern,
				RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
		}

		/// <summary>
		/// Возвращает структуру, инфо о наличии свободного места в байтах.
		/// </summary>
		/// <returns></returns>
		public Result GetAvailableBytes()
		{
			var client = new WebClient();
			var content = client.DownloadString(Address);

			var matchCollection = _regex.Matches(content);
			if (matchCollection.Count != 1)
			{
				throw new InvalidOperationException("Regex returned incorrect results");
			}

			var match = matchCollection[0];

			if (match.Groups.Count != 5)
			{
				throw new InvalidOperationException("Regex returned incorrect Groups");
			}

			if (match.Captures.Count != 1)
			{
				throw new InvalidOperationException("Regex returned incorrect Captures");
			}

			var free = match.Result("$1");
			var labelFree = match.Result("$2");
			var all = match.Result("$3");
			var labelAll = match.Result("$4");

			return new Result(ToBytes(ToNumber(free), labelFree), ToBytes(ToNumber(all), labelAll));
		}

		private static double ToBytes(double size, string label)
		{
			const string Msg = "Volume label is invalid: '{0}'";
			if (string.IsNullOrEmpty(label))
			{
				throw new ArgumentException(string.Format(Msg, label), "label");
			}

			switch (label.ToLower())
			{
				case "b":
					return size;
				case "kb":
					return size * Math.Pow(2, 10);
				case "mb":
					return size * Math.Pow(2, 20);
				case "gb":
					return size * Math.Pow(2, 30);
				case "tb":
					return size * Math.Pow(2, 40);
				case "pb":
					return size * Math.Pow(2, 50);
				default:
					throw new ArgumentException(string.Format(Msg, label), "label");
			}
		}

		private static double ToNumber(string str)
		{
			double res;
			var result = double.TryParse(str, NumberStyles.Float, CultureInfo.CurrentCulture, out res);
			if (!result)
			{
				result = double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out res);
			}
			if (!result)
			{
				throw new InvalidOperationException(string.Format("Failed to parse: '{0}'", str));
			}
			return res;
		}
	}
}