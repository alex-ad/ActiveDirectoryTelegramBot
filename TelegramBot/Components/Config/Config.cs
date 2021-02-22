using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Models;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config
{
	/// <summary>
	///		Чтение настроек приложения из файла-конфигурации
	/// </summary>
	internal class Config : Decorator, IConfig
	{
		public delegate void ConfigUpdated(Config config);

		private static Config _instance;
		private static ILogger _logger;
		private static IComponent[] _decorators;

		private Config()
		{
		}

		/// <value>Адрес сервера Active Directory в виде строки из DC (Domain Component)</value>
		public string DomainAddress { get; private set; }
		/// <value>Адрес сервера Active Directory в виде строки domain.com</value>
		public string ServerAddress { get; private set; }
		/// <value>Имя пользователя (логин)</value>
		public string UserName { get; private set; }
		/// <value>Пароль пользователя</value>
		public string UserPassword { get; private set; }
		/// <value>Список групп Active Directory, члены которых имеют право на получения данных через чат и получение оповещений</value>
		public List<string> AllowedAdGroups { get; private set; }
		/// <value>Токен бота</value>
		public string TelegramBotToken { get; private set; }
		/// <value>Список авторизованных пользователей бота</value>
		public List<TelegramUser> TelegramUsers { get; private set; }

		/// <summary>
		///		Чтение настроек приложения из файла Config.config
		/// </summary>
		/// <returns></returns>
		public bool TryGetParamsFromFile()
		{
			if (!File.Exists("Config.config"))
			{
				_logger?.Log("Config not found. Ensure, it is exists in the application directory.",
					OutputTarget.Console | OutputTarget.File);
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
			}
			catch (XPathException e)
			{
				_logger?.Log($"Error occured on reading params from Config: {e.Message}",
					OutputTarget.Console | OutputTarget.File);
				return false;
			}
			catch (XmlException e)
			{
				_logger?.Log($"Error occured on reading params Config: {e.Message}",
					OutputTarget.Console | OutputTarget.File);
				return false;
			}
			catch (IOException e)
			{
				_logger?.Log($"Error occured on reading file Config: {e.Message}",
					OutputTarget.Console | OutputTarget.File);
				return false;
			}
			catch (UnauthorizedAccessException e)
			{
				_logger?.Log($"Error occured on reading file Config: {e.Message}",
					OutputTarget.Console | OutputTarget.File);
				return false;
			}

			if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword) ||
			    string.IsNullOrEmpty(ServerAddress) || string.IsNullOrEmpty(TelegramBotToken))
			{
				_logger?.Log("Ensure, Config has correct parameters inside.", OutputTarget.Console | OutputTarget.File);
				return false;
			}

			OnConfigUpdated?.Invoke(this);
			return true;
		}

		/// <summary>
		///		Запись в файл-конфигурацию нового пользователя
		/// </summary>
		/// <param name="user"></param>
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

		/// <summary>
		///		Удаление пользователя из файла-конфигурации и списка TelegramUsers
		/// </summary>
		/// <param name="user"></param>
		public void RemoveUser(TelegramUser user)
		{
			TelegramUsers.Remove(user);

			var xFile = new XmlDocument();

			xFile.Load("Config.config");
			var xElement = xFile.DocumentElement;
			var xRoot = xElement?.SelectSingleNode("UsersAccessList");
			var userNode = xRoot?.SelectSingleNode($"User[TelegramId='{user.TelegramId}']");

			if (userNode == null) return;

			xRoot.RemoveChild(userNode);
			xFile.Save("Config.config");
		}

		public static event ConfigUpdated OnConfigUpdated;

		public static Config Instance()
		{
			_instance = _instance ?? new Config();
			return _instance;
		}

		/// <summary>
		///		Включение компонента
		/// </summary>
		/// <param name="decorators"></param>
		public override void Init(params IComponent[] decorators)
		{
			base.Init(decorators);

			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_logger?.Log("Initializing Service: Config...", OutputTarget.Console);
			TryGetParamsFromFile();
		}

		/// <summary>
		///		Чтение настроек Active Directory
		/// </summary>
		/// <param name="xElement"></param>
		private void GetAdParams(XmlElement xElement)
		{
			var xRoot = xElement.SelectSingleNode("AD");
			UserName = xRoot?.SelectSingleNode("UserName")?.InnerText.Trim();
			UserPassword = xRoot?.SelectSingleNode("UserPassword")?.InnerText.Trim();
			ServerAddress = xRoot?.SelectSingleNode("ServerName")?.InnerText.Trim();
			DomainAddress = string.Empty;

			if (string.IsNullOrEmpty(ServerAddress)) return;
			var parts = ServerAddress.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 1) return;
			foreach (var p in parts) DomainAddress += "DC=" + p + ",";

			DomainAddress = "LDAP://" + DomainAddress.Trim(',');
		}

		/// <summary>
		///		Чтение настроек бота
		/// </summary>
		/// <param name="xElement"></param>
		private void GetBotParams(XmlElement xElement)
		{
			var xRoot = xElement.SelectSingleNode("TelegramBot");
			TelegramBotToken = xRoot?.SelectSingleNode("Token")?.InnerText.Trim();
		}

		/// <summary>
		///		Чтение списка пользователей
		/// </summary>
		/// <param name="xElement"></param>
		private void GetUsersParams(XmlElement xElement)
		{
			var xRoot = xElement.SelectSingleNode("UsersAccessList");

			var groups = xRoot?.SelectNodes("//AllowedAdGroups/Group");
			AllowedAdGroups = new List<string>();

			if (groups != null)
				foreach (XmlNode g in groups)
					if (!string.IsNullOrEmpty(g.InnerText.Trim()))
						AllowedAdGroups.Add(g.InnerText.Trim());

			var users = xRoot?.SelectNodes("User");
			TelegramUsers = new List<TelegramUser>();

			if (users == null) return;

			foreach (XmlNode u in users)
				TelegramUsers.Add(new TelegramUser
				{
					ADUserName = u.SelectSingleNode("ADUserName")?.InnerText.Trim(),
					Allowed = bool.TryParse(u.SelectSingleNode("Allowed")?.InnerText.Trim(), out var allowed)
						? allowed
						: false,
					TelegramId = int.TryParse(u.SelectSingleNode("TelegramId")?.InnerText.Trim(), out var id)
						? id
						: default,
					Notifications = ParseAdNotifications(u)
				});
		}

		// TODO v2 Заготовка для будущего использования
		/// <summary>
		///		Расшифровка битовой маски, представляющей собой значение того, на какие именно оповещения подписан пользователь
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private AdNotifications ParseAdNotifications(XmlNode node)
		{
			return (bool.TryParse(node.SelectSingleNode("//Notifications/UserInfoChanged")?.InnerText.Trim(),
				       out var uic)
				       ? uic ? AdNotifications.UserInfoChanged : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/UserAdded")?.InnerText.Trim(), out var ua)
				       ? ua ? AdNotifications.UserAdded : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/UserBlockedUnblocked")?.InnerText.Trim(),
				       out var ubu)
				       ? ubu ? AdNotifications.UserBlockedUnblocked : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/UserDeleted")?.InnerText.Trim(), out var ud)
				       ? ud ? AdNotifications.UserDeleted : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/UserMoved")?.InnerText.Trim(), out var um)
				       ? um ? AdNotifications.UserMoved : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/ComputerInfoChanged")?.InnerText.Trim(),
				       out var cc)
				       ? cc ? AdNotifications.ComputerInfoChanged : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/ComputerAdded")?.InnerText.Trim(), out var ca)
				       ? ca ? AdNotifications.ComputerAdded : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/ComputerBlockedUnblocked")?.InnerText.Trim(),
				       out var cnu)
				       ? cnu ? AdNotifications.ComputerBlockedUnblocked : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/ComputerDeleted")?.InnerText.Trim(),
				       out var cd)
				       ? cd ? AdNotifications.ComputerDeleted : 0
				       : 0) &
			       (bool.TryParse(node.SelectSingleNode("//Notifications/ComputerMoved")?.InnerText.Trim(), out var cm)
				       ? cm ? AdNotifications.ComputerMoved : 0
				       : 0);
		}
	}
}