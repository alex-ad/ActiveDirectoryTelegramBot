using System;
using System.ServiceProcess;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot
{
	public partial class Service1 : ServiceBase
	{
		//readonly 

		public Service1()
		{
			InitializeComponent();
		}

		internal void TestStartupAndStop(string[] args)
		{
			//debug method only
			OnStart(args);
			Console.ReadLine();
			OnStop();
		}

		protected override void OnStart(string[] args)
		{
			StartUp.Initialize();
		}

		protected override void OnStop()
		{
		}
	}
}