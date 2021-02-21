using System;
using System.ServiceProcess;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot
{
	internal static class Program
	{
		/// <summary>
		///     Главная точка входа для приложения.
		/// </summary>
		private static void Main(string[] args)
		{
			//debug begin
			if (Environment.UserInteractive)
			{
				var service1 = new Service1();
				service1.TestStartupAndStop(args);
			}
			else
			{
				ServiceBase[] servicesToRun = {new Service1()};
				ServiceBase.Run(servicesToRun);
			}
			//debug end

			/*ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new Service1()
			};
			ServiceBase.Run(ServicesToRun);*/
		}
	}
}