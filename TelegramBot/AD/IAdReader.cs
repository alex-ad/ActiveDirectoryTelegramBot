using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
	internal interface IAdReader
	{
		AdReader Request { get; }
		void Connect();
		UserPrincipal GetUserObjectByLogin(string accountName);
		string GetUserProperty(UserPrincipal userPrincipal, string propertyName);
		IEnumerable<string> GetGroupsByUser(UserPrincipal userPrincipal);
		UserPrincipal GetUserObjectByName(string fullName);
		bool IsIdentifiedUser(string userName, string userPassword, List<string> groups);
	}
}
