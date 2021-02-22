using System;
using System.IO;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger
{
	[Flags]
	public enum OutputTarget
	{
		File = 1,
		Console = 2
	}

	/// <summary>
	///		Вывод логов в консоль или(и) файл
	/// </summary>
	internal class Logger : Decorator, ILogger
	{
		private static Logger _instance;

		private static readonly string _fileName = "log.txt";
		private static DateTime _time;

		private Logger()
		{
		}

		/// <summary>
		///		Вывод логов в консоль или(и) файл
		/// </summary>
		/// <param name="message">Текстовое сообщение</param>
		/// <param name="outputTarget">Вывод: консоль, файл</param>
		public void Log(string message, OutputTarget outputTarget)
		{
			_time = DateTime.Now;
			if ((outputTarget & OutputTarget.File) != 0) LogToFile(message);
			if ((outputTarget & OutputTarget.Console) != 0) LogToConsole(message);
		}

		public static Logger Instance()
		{
			_instance = _instance ?? new Logger();
			return _instance;
		}

		public override void Init(params IComponent[] decorators)
		{
			base.Init(decorators);
			Log("Initializing Service: Logger...", OutputTarget.Console);
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