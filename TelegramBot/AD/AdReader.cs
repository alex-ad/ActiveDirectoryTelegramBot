using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
	internal class AdReader
	{
		private static PrincipalContext _adContext;

		public AdReader(PrincipalContext adContext)
		{
			_adContext = adContext;
		}

		public string GetUserProperty(UserPrincipal userPrincipal, string propertyName) =>
			_adContext.GetUserProperty(userPrincipal, propertyName);

		// зачем 1...
		IEnumerable<string> GetGroupsByUserObject(UserPrincipal userPrincipal) =>
			_adContext.GetGroupNamesByUserObject(userPrincipal);

		public IEnumerable<string> GetUserNamesByGroupObject(GroupPrincipal groupPrincipal) =>
			_adContext.GetUserNamesByGroupObject(groupPrincipal);

		public UserPrincipal GetUserObjectByLogin(string accountName) => _adContext.GetUserObjectByLogin(accountName);
		
		public UserPrincipal GetUserObjectByName(string fullName) => _adContext.GetUserObjectByName(fullName);

		// ... и 2, сделать все одним методом
		public IEnumerable<string> GetGroupsByUser(UserPrincipal userPrincipal) => GetGroupsByUserObject(userPrincipal);

		//public bool IsIdentifiedUser(string userName, List<string> groups) => _adContext.IsIdentifiedUser(userName, groups);

		public bool IsIdentifiedUser(string userName, string userPassword, List<string> groups) => _adContext.IsIdentifiedUser(userName, userPassword, groups);

		public ComputerPrincipal GetComputerObjectByName(string computerName) =>
			_adContext.GetComputerObjectByName(computerName);

		public GroupPrincipal GetGroupObjectByName(string groupName) => _adContext.GetGroupObjectByName(groupName);
	}
}
