using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD
{
	/// <summary>
	///		Получение объектов из Active Directory
	/// </summary>
	internal class AdReader
	{
		private static PrincipalContext _adContext;

		/// <summary>
		///		Получение объектов из Active Directory
		/// </summary>
		/// <param name="adContext">Контекст Active Directory</param>
		public AdReader(PrincipalContext adContext)
		{
			_adContext = adContext;
		}

		public string GetUserProperty(UserPrincipal userPrincipal, string propertyName)
		{
			if (!(userPrincipal.GetUnderlyingObject() is DirectoryEntry de)) return string.Empty;
			return de.Properties.Contains(propertyName) ? de.Properties[propertyName].Value.ToString() : string.Empty;
		}

		public string GetComputerProperty(ComputerPrincipal computerPrincipal, string propertyName)
		{
			if (!(computerPrincipal.GetUnderlyingObject() is DirectoryEntry de)) return string.Empty;
			return de.Properties.Contains(propertyName) ? de.Properties[propertyName].Value.ToString() : string.Empty;
		}

		public IEnumerable<string> GetGroupsByUserObject(UserPrincipal userPrincipal) =>
			userPrincipal.GetGroups(_adContext).Select(x => x.Name);

		public IEnumerable<string> GetUserNamesByGroupObject(GroupPrincipal groupPrincipal) =>
			groupPrincipal.GetMembers(true).Select(x => x.Name);

		public UserPrincipal GetUserObjectByLogin(string accountName) =>
			UserPrincipal.FindByIdentity(_adContext, IdentityType.SamAccountName, accountName) ??
			throw new NoMatchingPrincipalException("Sorry, no such user account found for the domain");

		public UserPrincipal GetUserObjectByName(string fullName) =>
			UserPrincipal.FindByIdentity(_adContext, IdentityType.Name, fullName) ??
			throw new NoMatchingPrincipalException("Sorry, no such user account found for the domain");

		public bool IsIdentifiedUser(string userName, string userPassword, List<string> groups)
		{
			var user = UserPrincipal.FindByIdentity(_adContext, IdentityType.SamAccountName, userName);
			if (user == null)
				return false;

			if (user.IsAccountLockedOut())
				return false;

			if (!_adContext.ValidateCredentials(userName, userPassword))
				return false;

			return IsUserMultiGroupMember(user, groups);
		}

		public ComputerPrincipal GetComputerObjectByName(string computerName) =>
			ComputerPrincipal.FindByIdentity(_adContext, IdentityType.Name, computerName) ??
			throw new NoMatchingPrincipalException("Sorry, no such computer account found for the domain");

		public GroupPrincipal GetGroupObjectByName(string groupName) =>
			GroupPrincipal.FindByIdentity(_adContext, IdentityType.Name, groupName) ??
			throw new NoMatchingPrincipalException("Sorry, no such group name found for the domain");

		private bool IsUserMultiGroupMember(UserPrincipal userPrincipal, List<string> groupsList)
		{
			if (groupsList == null || groupsList.Count < 1)
				return true;
			var result = false;

			foreach (var g in groupsList)
				using (var groupPrincipal = GetGroupObjectByName(g.Trim()))
				{
					if (userPrincipal != null && groupPrincipal != null)
					{
						result = groupPrincipal.Members.Contains(userPrincipal);
						if (result)
							break;
					}
				}

			return result;
		}
	}
}