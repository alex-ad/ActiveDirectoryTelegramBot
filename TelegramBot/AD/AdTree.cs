using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.CompilerServices;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
    public static class AdTree
    {
        public static UserPrincipal GetUserObjectByLogin(this PrincipalContext principalContext, string accountName) =>
            UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, accountName) ??
            throw new NoMatchingPrincipalException($"Sorry, no such user account found for the domain");

		public static UserPrincipal GetUserObjectByName(this PrincipalContext principalContext, string fullName) =>
			UserPrincipal.FindByIdentity(principalContext, IdentityType.Name, fullName) ??
			throw new NoMatchingPrincipalException($"Sorry, no such user account found for the domain");

		/*public static void AddUserToGroup(this PrincipalContext principalContext, string accountName, string groupName)
        {
            var userPrincipal = principalContext.GetUserObjectByLogin(accountName);
            var groupPrincipal = principalContext.GetGroupObjectByGroupName(groupName);

            if (userPrincipal != null && groupPrincipal != null)
            {
                if (!principalContext.IsUserMultiGroupMember(accountName, groupName))
                {
                    groupPrincipal.Members.Add(userPrincipal);
                    groupPrincipal.Save();
                }
            }
        }*/

		/*public static bool IsUserMultiGroupMember(this PrincipalContext principalContext, string sUserName, List<string> sGroupName)
		{
			if (sGroupName?.Count < 1) return true;
			bool bResult = false;

			for ( int i = 0; i < sGroupName?.Count; i++ )
			{
				using ( UserPrincipal oUserPrincipal = principalContext.GetUserObjectByLogin(sUserName) )
				using ( GroupPrincipal oGroupPrincipal = principalContext.GetGroupObjectByGroupName(sGroupName[i].Trim()) )
				{
					if ( oUserPrincipal != null && oGroupPrincipal != null )
					{
						bResult = oGroupPrincipal.Members.Contains(oUserPrincipal);
						if ( bResult ) break;
					}
				}
			}
			return bResult;
		}*/

		public static bool IsUserMultiGroupMember(this PrincipalContext principalContext, UserPrincipal userPrincipal, List<string> groupsList)
		{
			if (groupsList == null || groupsList.Count < 1) return true;
			var result = false;

			foreach (var g in groupsList )
			{
				using ( var groupPrincipal = principalContext.GetGroupObjectByGroupName(g.Trim()) )
				{
					if ( userPrincipal != null && groupPrincipal != null )
					{
						result = groupPrincipal.Members.Contains(userPrincipal);
						if ( result )
							break;
					}
				}
				
			}

			return result;
		}

        public static bool IsIdentifiedUser(this PrincipalContext principalContext, string userName, List<string> groups)
		{
			var user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userName);
			if (user == null) return false;

			if (user.IsAccountLockedOut()) return false;

			return IsUserMultiGroupMember(principalContext, user, groups);
		}

        public static bool IsIdentifiedUser(this PrincipalContext principalContext, string userName, string userPassword, List<string> groups)
        {
			var user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, userName);
	        if ( user == null )
		        return false;

	        if ( user.IsAccountLockedOut() )
		        return false;

	        if ( !principalContext.ValidateCredentials(userName, userPassword) )
		        return false;

			return IsUserMultiGroupMember(principalContext, user, groups);
        }

		/*public static IEnumerable<string> GetUserNamesByGroupName(this PrincipalContext principalContext, string groupName)
        {
            var gp = principalContext.GetGroupObjectByGroupName(groupName);
            if (gp == null) yield break;
            
            foreach (var u in gp.Members)
            {
                yield return u.DisplayName;
            }
        }*/

		public static IEnumerable<UserInfo> GetUserObjectsByGroupName(this PrincipalContext principalContext, string groupName)
        {
			var gp = principalContext.GetGroupObjectByGroupName(groupName);
            if (gp == null) yield break;

            foreach (var u in gp.Members)
            {
                var user = u as UserPrincipal;
                if (user == null) yield break;
                yield return new UserInfoExt
                {
                    Name = user.DisplayName,
                    SamAccountName = user.SamAccountName,
                    Sid = user.Sid.ToString(),
                    Enabled = user.Enabled ?? false
                };
            }
        }

		public static string GetUserProperty(this PrincipalContext principalContext, UserPrincipal userPrincipal, string propertyName)
		{
			var de = userPrincipal.GetUnderlyingObject() as DirectoryEntry;
			if ( de == null )
				return string.Empty;
			return de.Properties.Contains(propertyName) ? de.Properties[propertyName].Value.ToString() : string.Empty;
		}

		public static IEnumerable<string> GetGroupNamesByUserObject(this PrincipalContext principalContext,
            UserPrincipal userPrincipal) => userPrincipal.GetGroups(principalContext).Select(x => x.Name);
        
        public static GroupPrincipal GetGroupObjectByGroupName(this PrincipalContext principalContext, string groupName) =>
            GroupPrincipal.FindByIdentity(principalContext, groupName);

        public static PrincipalSearchResult<Principal> GetUserObjectsCollection(this PrincipalContext principalContext) =>
            new PrincipalSearcher(new UserPrincipal(principalContext)).FindAll();

        public static PrincipalSearchResult<Principal> GetGroupObjectsCollection(this PrincipalContext principalContext) =>
            new PrincipalSearcher(new GroupPrincipal(principalContext)).FindAll();
    }
}
