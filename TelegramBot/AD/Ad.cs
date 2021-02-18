using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
	internal class Ad : Decorator, IAdReader
	{
		private static IComponent[] _decorators;
		private static ILogger _logger;
		private static AdReader _ad;
		private static AdConnection _adConnection;
		private static PrincipalContext _adContext;
		private static Ad _instance;
		private static IConfig _config;

		public AdReader Request => _ad;

		private Ad() { }

		public static Ad Instance()
		{
			_instance = _instance ?? new Ad();
			return _instance;
		}

		public override void Init(params IComponent[] decorators)
		{
			base.Init(decorators);

			HelpMsg.HelpMsg.MsgList.Add("/UserByLogin [/ul][/u] - Get user data by AccountName");
			HelpMsg.HelpMsg.MsgList.Add("/UserByName [/un] - Get user data by FullName (DisplayName)");
			HelpMsg.HelpMsg.MsgList.Add("/Group [/g] - Get group data by GroupName");
			HelpMsg.HelpMsg.MsgList.Add("/Computer [/c] - Get computer data by ComputerName");

			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = _decorators?.OfType<IConfig>().FirstOrDefault();
			_logger?.Log("Initializing Service: Active Directory...", OutputTarget.Console);

			Config.Config.OnConfigUpdated += Config_OnConfigUpdated;
			Connect();
		}

		private void Config_OnConfigUpdated(IConfig config)
		{
			_config = config;
			Connect();
		}

		public void Connect()
		{
			_adConnection = AdConnection.Instance(_logger, _config);
			if ( _adConnection != null )
				if ( _adConnection.TryConnect(out _adContext) )
					_ad = new AdReader(_adContext);
		}

		public UserPrincipal GetUserObjectByLogin(string accountName) => _ad.GetUserObjectByLogin(accountName);

		public string GetUserProperty(UserPrincipal userPrincipal, string propertyName) =>
			_ad.GetUserProperty(userPrincipal, propertyName);

		public IEnumerable<string> GetGroupsByUser(UserPrincipal userPrincipal) => _ad.GetGroupsByUser(userPrincipal);

		public IEnumerable<string> GetUserNamesByGroupObject(GroupPrincipal groupPrincipal) =>
			_ad.GetUserNamesByGroupObject(groupPrincipal);

		public UserPrincipal GetUserObjectByName(string fullName) => _ad.GetUserObjectByName(fullName);

		public bool IsIdentifiedUser(string userName, string userPassword, List<string> groups) => _ad.IsIdentifiedUser(userName, userPassword, groups);

		public ComputerPrincipal GetComputerObjectByName(string computerName) =>
			_ad.GetComputerObjectByName(computerName);

		public GroupPrincipal GetGroupObjectByName(string groupName) => _ad.GetGroupObjectByName(groupName);
	}
}
