using System;
using System.IO;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Logger
{
	[Flags]
	public enum OutputTarget
	{
		File = 1,
		Console = 2
	}

	// TODO Назначение вывода логов брать из конфига
	public class Logger : Decorator, ILogger
	{
		private static Logger _instance;

		private static string _fileName = "log.txt";
		private static DateTime _time;

		protected Logger() { }

		public static Logger Instance(params IComponent[] decorators)
		{
			_instance = _instance ?? new Logger();
			return _instance;
		}

		public override void Init()
		{
			Log("Initialize Service: Logger", OutputTarget.Console);
			base.Init();
		}

		public void Log(string message, OutputTarget outputTarget)
		{
			_time = DateTime.Now;
			if ((outputTarget & OutputTarget.File) != 0) LogToFile(message);
			if ((outputTarget & OutputTarget.Console) != 0) LogToConsole(message);
		}

		private static void LogToConsole(string message)
		{
			Console.WriteLine($"{_time.ToString("yyyy.mm.dd HH:mm:ss")}: {message}");
		}

		private static void LogToFile(string message)
		{
			using (var f = File.AppendText(_fileName))
			{
				f.WriteLine($"{_time.ToString("yyyy.mm.dd HH:mm:ss")}: {message}");
			}
		}
	}
}
