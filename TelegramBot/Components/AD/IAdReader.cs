using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD
{
	internal interface IAdReader
	{
		void Connect();
		UserPrincipal GetUserObjectByLogin(string accountName);
		string GetUserProperty(UserPrincipal userPrincipal, string propertyName);
		IEnumerable<string> GetGroupsByUserObject(UserPrincipal userPrincipal);
		UserPrincipal GetUserObjectByName(string fullName);
		ComputerPrincipal GetComputerObjectByName(string computerName);
		bool IsIdentifiedUser(string userName, string userPassword, List<string> groups);
		GroupPrincipal GetGroupObjectByName(string groupName);
		IEnumerable<string> GetUserNamesByGroupObject(GroupPrincipal groupPrincipal);
	}
}
