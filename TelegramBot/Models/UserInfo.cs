using System;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Models
{
    public class UserInfo : ObjectInfo
    {
	    public bool Enabled { get; set; }
	    public string Company { get; set; }
	    public string Department { get; set; }
	    public string Title { get; set; }
	    public string Mail { get; set; }
	    public string TelephoneNumber { get; set; }
	    public DateTime LastLogon { get; set; }
	    public string Description { get; set; }
		public string SamAccountName { get; set; }
    }
}