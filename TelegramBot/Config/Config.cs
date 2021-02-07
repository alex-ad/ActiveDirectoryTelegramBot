using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Config
{
	public class Config : Decorator, IConfig
	{
		public string ServerAddress { get; private set; }
		public string DomainAddress { get; private set; }
		public string UserName { get; private set; }
		public string UserPassword { get; private set; }
		public List<string> AllowedAdGroups { get; private set; }
		public string TelegramBotToken { get; private set; }
		public List<TelegramUser> TelegramUsers { get; private set; }

		private static Config _instance;
		private static ILogger _logger;
		private static IComponent[] _decorators;

		protected Config() { }

		public static Config Instance(params IComponent[] decorators)
		{
			_instance = _instance ?? new Config();
			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			return _instance;
		}

		public override void Init()
		{
			base.Init();
			_logger?.Log("Initialize Service: Config", OutputTarget.Console);
		}

		public bool TryGetParamsFromFile()
		{
			if ( !File.Exists("Config.config") )
			{
				_logger?.Log("Config.config not found. Ensure, it is exists in the application directory.", OutputTarget.Console & OutputTarget.File);
				return false;
			}

			var xFile = new XmlDocument();
			try
			{
				xFile.Load("Config.config");
				var xElement = xFile.DocumentElement;

				GetAdParams(xElement);
				GetBotParams(xElement);
				GetUsersParams(xElement);

			} catch (XPathException e)
			{
				_logger?.Log($"Error occured on reading params from Config.config: {e.Message}", OutputTarget.Console & OutputTarget.File);
				return false;
			} catch (XmlException e)
			{
				_logger?.Log($"Error occured on reading params Config.config: {e.Message}", OutputTarget.Console & OutputTarget.File);
				return false;
			} catch (IOException e)
			{
				_logger?.Log($"Error occured on reading file Config.config: {e.Message}", OutputTarget.Console & OutputTarget.File);
				return false;
			} catch (UnauthorizedAccessException e)
			{
				_logger?.Log($"Error occured on reading file Config.config: {e.Message}", OutputTarget.Console & OutputTarget.File);
				return false;
			}

			if ( string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword) ||
			     string.IsNullOrEmpty(ServerAddress) || string.IsNullOrEmpty(TelegramBotToken) )
			{
				_logger?.Log($"Ensure, Config.config has correct parameters inside.", OutputTarget.Console & OutputTarget.File);
				return false;
			}

			return true;
		}

		public void SetNewUser(TelegramUser user)
		{
			var xFile = new XmlDocument();

			xFile.Load("Config.config");
			var xElement = xFile.DocumentElement;
			var xRoot = xElement?.SelectSingleNode("UsersAccessList");

			var userNode = xFile.CreateElement("User");
			var userId = xFile.CreateElement("TelegramId");
			var userName = xFile.CreateElement("ADUserName");
			var userAllowed = xFile.CreateElement("Allowed");
			var userNotifications = xFile.CreateElement("Notifications");

			var userIdValue = xFile.CreateTextNode(user.TelegramId.ToString());
			var userNameValue = xFile.CreateTextNode(user.ADUserName);
			var userAllowedValue = xFile.CreateTextNode(user.Allowed.ToString());

			userId.AppendChild(userIdValue);
			userName.AppendChild(userNameValue);
			userAllowed.AppendChild(userAllowedValue);

			userNode.AppendChild(userId);
			userNode.AppendChild(userName);
			userNode.AppendChild(userAllowed);
			userNode.AppendChild(userNotifications);

			xRoot?.AppendChild(userNode);

			xFile.Save("Config.config");
		}

		public void RemoveUser(TelegramUser user)
		{
			var xFile = new XmlDocument();

			xFile.Load("Config.config");
			var xElement = xFile.DocumentElement;
			var xRoot = xElement?.SelectSingleNode($"UsersAccessList");
			var userNode = xRoot.SelectSingleNode($"User[TelegramId='{user.TelegramId}']");

			if (userNode == null) return;

			xRoot.RemoveChild(userNode);
			xFile.Save("Config.config");
		}

		private void GetAdParams(XmlElement xElement)
		{
			var xRoot = xElement.SelectSingleNode("AD");
			UserName = xRoot.SelectSingleNode("UserName")?.InnerText.Trim();
			UserPassword = xRoot.SelectSingleNode("UserPassword")?.InnerText.Trim();
			ServerAddress = xRoot.SelectSingleNode("ServerName")?.InnerText.Trim();
			DomainAddress = String.Empty;
			
			if (string.IsNullOrEmpty(ServerAddress)) return;
			var parts = ServerAddress.Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 1) return;
			foreach (var p in parts)
			{
				DomainAddress += "DC=" + p + ",";
			}

			DomainAddress = "LDAP://" + DomainAddress.Trim(new[] {','});
		}

		private void GetBotParams(XmlElement xElement)
		{
			var xRoot = xElement.SelectSingleNode("TelegramBot");
			TelegramBotToken = xRoot.SelectSingleNode("Token")?.InnerText.Trim();
		}

		private void GetUsersParams(XmlElement xElement)
		{
			var xRoot = xElement.SelectSingleNode("UsersAccessList");

			var groups = xRoot.SelectNodes("//AllowedAdGroups/Group");
			AllowedAdGroups = new List<string>();

			if (groups != null)
			{
				foreach (XmlNode g in groups)
				{
					if (!string.IsNullOrEmpty(g.InnerText.Trim()))
						AllowedAdGroups.Add(g.InnerText.Trim());
				}
			}

			var users = xRoot.SelectNodes("User");
			TelegramUsers = new List<TelegramUser>();

			if (users == null) return;

			foreach (XmlNode u in users)
			{
				TelegramUsers.Add(new TelegramUser
				{
					ADUserName = u.SelectSingleNode("ADUserName")?.InnerText.Trim(),
					Allowed = bool.TryParse(u.SelectSingleNode("Allowed")?.InnerText.Trim(), out bool allowed) ? allowed : false,
					TelegramId = int.TryParse(u.SelectSingleNode("TelegramId")?.InnerText.Trim(), out int id) ? id : default,
					Notifications = ParseAdNotifications(u)
				});
			}
		}

		private AdNotifications ParseAdNotifications(XmlNode node)
		{
			return (bool.TryParse(node.SelectSingleNode("//Notifications/UserInfoChanged")?.InnerText.Trim(), out bool uic) ? uic ? AdNotifications.UserInfoChanged : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/UserAdded")?.InnerText.Trim(), out bool ua) ? ua ? AdNotifications.UserAdded : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/UserBlockedUnblocked")?.InnerText.Trim(), out bool ubu) ? ubu ? AdNotifications.UserBlockedUnblocked : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/UserDeleted")?.InnerText.Trim(), out bool ud) ? ud ? AdNotifications.UserDeleted : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/UserMoved")?.InnerText.Trim(), out bool um) ? um ? AdNotifications.UserMoved : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/ComputerInfoChanged")?.InnerText.Trim(), out bool cc) ? cc ? AdNotifications.ComputerInfoChanged : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/ComputerAdded")?.InnerText.Trim(), out bool ca) ? ca ? AdNotifications.ComputerAdded : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/ComputerBlockedUnblocked")?.InnerText.Trim(), out bool cnu) ? cnu ? AdNotifications.ComputerBlockedUnblocked : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/ComputerDeleted")?.InnerText.Trim(), out bool cd) ? cd ? AdNotifications.ComputerDeleted : 0 : 0) &
				(bool.TryParse(node.SelectSingleNode("//Notifications/ComputerMoved")?.InnerText.Trim(), out bool cm) ? cm ? AdNotifications.ComputerMoved : 0 : 0);
		}

		public void Reload()
		{
			throw new NotImplementedException();
		}
	}
}
