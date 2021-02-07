using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
			this.OnStart(args);
			Console.ReadLine();
			this.OnStop();
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
