using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot
{
	static class Program
	{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		static void Main(string[] args)
		{
			//debug begin
			if ( Environment.UserInteractive )
			{
				Service1 service1 = new Service1();
				service1.TestStartupAndStop(args);
			} else
			{
				ServiceBase[] servicesToRun = { new Service1() };
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
