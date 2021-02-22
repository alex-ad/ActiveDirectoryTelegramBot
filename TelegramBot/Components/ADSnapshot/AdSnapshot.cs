using System;
using System.DirectoryServices;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Modules;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///     Отслеживание изменений в Active Directory
	/// </summary>
	public class AdSnapshot : Decorator, IAdSnapshot
	{
		public delegate void AdChanged();

		private static DirectoryEntry _directoryEntry;
		private static DirectorySearcher _directorySearcher;
		private static AdNotifyCollection _adNotifyCollection;
		private static byte[] _cookie;
		private static AdSnapshot _instance;
		private static bool _enabled;
		private static bool? _connected;

		private static IConfig _config;
		private static ILogger _logger;
		private static IComponent[] _decorators;

		private readonly CancellationTokenSource _cancellationTokenSource;
		private CancellationToken _cancellationToken;

		private AdSnapshot()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_cancellationToken = _cancellationTokenSource.Token;
		}

		/// <summary>
		///     Запуск цикла чтения изменений в Active Directory
		/// </summary>
		/// <param name="loopPeriodInMilliseconds"></param>
		public async void RunAsync(int loopPeriodInMilliseconds)
		{
			AdInit();
			if ((bool) !_connected)
				return;
			InitializeSnapshot();
			_enabled = true;

			await Task.Run(() =>
			{
				while (_enabled)
				{
					if (_cancellationToken.IsCancellationRequested)
					{
						_enabled = false;
						break;
					}

					CompareSnapshot();
					Thread.Sleep(loopPeriodInMilliseconds);
				}
			}, _cancellationToken);
		}

		public static event AdChanged OnAdChanged;

		/// <summary>
		///     Включение компонента
		/// </summary>
		/// <param name="decorators"></param>
		public override void Init(params IComponent[] decorators)
		{
			base.Init(decorators);

			HelpMessage.MsgList.Add(
				"/NotificationsOn [/non] -uUSER_AD_ACCOUNT_NAME -pUSER_AD_PASSWORD : Subscribe to notifications on AD changes");
			HelpMessage.MsgList.Add("/NotificationsOff [/nof] : Unsubscribe to notifications on AD changes");

			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = (Config.Config) _decorators?.OfType<IConfig>().FirstOrDefault();
			_logger?.Log("Initializing Service: Active Directory Snapshot...", OutputTarget.Console);

			if (string.IsNullOrEmpty(_config?.ServerAddress) || string.IsNullOrEmpty(_config?.UserName) ||
			    string.IsNullOrEmpty(_config?.UserPassword))
				return;

			Config.Config.OnConfigUpdated += Config_OnConfigUpdated;
			RunAsync(3000);
			AdNotifySender.Instance();
		}

		public static AdSnapshot Instance()
		{
			_instance = _instance ?? new AdSnapshot();
			_adNotifyCollection = _adNotifyCollection ?? new AdNotifyCollection();
			_connected = null;
			return _instance;
		}

		/// <summary>
		///     Создание подключения к Active Directory и получение первичных куки для последующего определения изменений
		/// </summary>
		private static void AdInit()
		{
			if (_connected == null || (bool) !_connected)
				try
				{
					_directoryEntry = _directoryEntry ?? new DirectoryEntry("LDAP://" + _config.ServerAddress,
						                  _config.UserName, _config.UserPassword);
					_directorySearcher = _directorySearcher ?? new DirectorySearcher(_directoryEntry)
					{
						Filter =
							"(|(isDeleted=true)(&(objectClass=user)(objectCategory=person))(&(objectClass=computer)(objectCategory=computer))(&(objectClass=group)(objectCategory=group)))",
						SearchScope = SearchScope.Subtree,
						ExtendedDN = ExtendedDN.Standard,
						Tombstone = true
					};

					_directorySearcher.DirectorySynchronization =
						new DirectorySynchronization(DirectorySynchronizationOptions.ObjectSecurity);

					foreach (SearchResult _ in _directorySearcher.FindAll())
					{
					}

					_connected = true;
				}
				catch
				{
					_connected = false;
					_logger.Log("Some error occured on Active Directory connecting",
						OutputTarget.Console | OutputTarget.File);
				}
		}

		/// <summary>
		///     Получение куки, изменение которых говорит об изменении в Active Directory
		/// </summary>
		private static void InitializeSnapshot()
		{
			_cookie = _directorySearcher.DirectorySynchronization.GetDirectorySynchronizationCookie();
		}

		/// <summary>
		///     Сравнение предыдущего и текущего состояния Active Directory на основе куки
		/// </summary>
		private static void CompareSnapshot()
		{
			var dirSync = new DirectorySynchronization(DirectorySynchronizationOptions.ObjectSecurity, _cookie);

			_directorySearcher = new DirectorySearcher(_directoryEntry)
			{
				DirectorySynchronization = dirSync
			};

			foreach (SearchResult res in _directorySearcher.FindAll())
			{
				var delta = res?.Properties;
				if (delta?.PropertyNames == null)
					continue;
				var found = false;
				foreach (string prop in delta.PropertyNames)
					switch (prop.ToLower())
					{
						case "objectguid":
						case "adspath":
						case "instancetype":
						case "whenchanged":
						case "lastlogontimestamp":
						case "lastlogon":
						case "pwdlastset":
						case "distinguishedname":
							break;
						case "cn":
						case "name":
						case "samaccountname":
						case "samaccounttype":
						case "description":
						case "operatingsystem":
						case "primarygroupid":
						case "useraccountcontrol":
						case "company":
						case "department":
						case "displayname":
						case "givenname":
						case "l":
						case "mail":
						case "memberof":
						case "member":
						case "physicaldeliveryofficename":
						case "sn":
						case "telephonenumber":
						case "title":
						case "userworkstations":
						case "parentguid":
							if (delta.Contains("isdeleted") &&
							    bool.TryParse(delta["isdeleted"][0].ToString(), out var deleted) && deleted)
							{
								ProcessDeleted(delta["distinguishedname"][0].ToString());
								found = true;
							}
							else if (delta.Contains("parentguid") &&
							         bool.TryParse(delta["parentguid"][0].ToString(), out var moved) && moved)
							{
								ProcessMoved(delta["distinguishedname"][0].ToString());
								found = true;
							}
							else
							{
								ProcessChanged(res.GetDirectoryEntry(), prop, delta);
								found = true;
							}

							break;
					}

				if (found)
					OnAdChanged?.Invoke();
				_directorySearcher.DirectorySynchronization.ResetDirectorySynchronizationCookie(_cookie);
				InitializeSnapshot();
			}
		}

		/// <summary>
		///     Отправка сообщения о перемещении объекта
		/// </summary>
		/// <param name="distinguishedName"></param>
		private static void ProcessMoved(string distinguishedName)
		{
			_adNotifyCollection.Push(CreateNotifyMessage("moved", distinguishedName));
		}

		/// <summary>
		///     Отправка сообщения об удалении объекта
		/// </summary>
		/// <param name="distinguishedName"></param>
		private static void ProcessDeleted(string distinguishedName)
		{
			_adNotifyCollection.Push(CreateNotifyMessage("deleted", distinguishedName));
		}

		/// <summary>
		///     Отправка сообщения об изменении объекта
		/// </summary>
		/// <param name="directoryEntry"></param>
		/// <param name="prop">Строковое значения имени изменившегося поля</param>
		/// <param name="delta">Список полей изменившегося объекта</param>
		private static void ProcessChanged(DirectoryEntry directoryEntry, string prop, ResultPropertyCollection delta)
		{
			var schemeClass = directoryEntry.SchemaClassName ?? string.Empty;

			if (!schemeClass.Equals("computer", StringComparison.OrdinalIgnoreCase)
			    && !schemeClass.Equals("user", StringComparison.OrdinalIgnoreCase)
			    && !schemeClass.Equals("group", StringComparison.OrdinalIgnoreCase))
				return;

			foreach (var val in delta[prop])
			{
				var name = directoryEntry.Name?.Substring(3) ?? string.Empty;
				var parent = directoryEntry.Parent.Name ?? string.Empty;
				if (name.Equals(val.ToString(), StringComparison.OrdinalIgnoreCase))
					continue;
				_adNotifyCollection.Push(CreateNotifyMessage(schemeClass, name, prop, val, parent));
			}
		}

		/// <summary>
		///     Создание события-оповещения об изменениях
		/// </summary>
		/// <param name="schemeClass"></param>
		/// <param name="name"></param>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		private static AdNotifyMessage CreateNotifyMessage(string schemeClass, string name, string property = null,
			object value = null, string parent = null)
		{
			string val;
			if (value is string)
				val = value.ToString();
			else if (value is byte[])
				val = parent;
			else
				val = value?.ToString() ?? string.Empty;

			return new AdNotifyMessage(schemeClass, name, property, val);
		}

		private void Stop()
		{
			_cancellationTokenSource.Cancel();
			_connected = null;
		}

		/// <summary>
		///     Перезапуск слушателя изменений при обновлении конфигурации приложения
		/// </summary>
		/// <param name="config"></param>
		private void Config_OnConfigUpdated(IConfig config)
		{
			_config = config;

			if (string.IsNullOrEmpty(_config?.ServerAddress) || string.IsNullOrEmpty(_config?.UserName) ||
			    string.IsNullOrEmpty(_config?.UserPassword))
				return;

			Stop();
			Thread.Sleep(1000);
			RunAsync(3000);
			AdNotifySender.Instance();
		}
	}
}