using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
	public class AdReader
	{
		private static PrincipalContext _adContext;

		public AdReader(PrincipalContext adContext)
		{
			_adContext = adContext;
		}

		public string GetUserProperty(UserPrincipal userPrincipal, string propertyName) =>
			_adContext.GetUserProperty(userPrincipal, propertyName);

		IEnumerable<string> GetGroupsByUserObject(UserPrincipal userPrincipal) =>
			_adContext.GetGroupNamesByUserObject(userPrincipal);

		public UserPrincipal GetUserObjectByLogin(string accountName) => _adContext.GetUserObjectByLogin(accountName);
		
		public UserPrincipal GetUserObjectByName(string fullName) => _adContext.GetUserObjectByName(fullName);

		public IEnumerable<string> GetGroupsByUser(UserPrincipal userPrincipal) => GetGroupsByUserObject(userPrincipal);

		public bool IsIdentifiedUser(string userName, List<string> groups) => _adContext.IsIdentifiedUser(userName, groups);

		public bool IsIdentifiedUser(string userName, string userPassword, List<string> groups) => _adContext.IsIdentifiedUser(userName, userPassword, groups);
	}
}
