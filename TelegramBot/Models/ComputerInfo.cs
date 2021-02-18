using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Models
{
	public class ComputerInfo : ObjectInfo
	{
		public bool Enabled { get; set; }
		public string Description { get; set; }
		public DateTime LastLogon { get; set; }
	}
}
