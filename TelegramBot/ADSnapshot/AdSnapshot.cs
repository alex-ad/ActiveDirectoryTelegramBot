using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Bot;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Service;


namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public class AdSnapshot : Decorator, IAdSnapshot
	{
		public delegate void AdChanged();
		public static event AdChanged OnAdChanged;

		private static DirectoryEntry _directoryEntry;
		private static DirectorySearcher _directorySearcher;
		//private static AdNotifyCollection _adNotifyCollection;
		private static byte[] _cookie;
		private static AdSnapshot _instance;
		private static bool _enabled;
		private static bool? _connected;

		private CancellationTokenSource _cancellationTokenSource;
		private CancellationToken _cancellationToken;
		
		private static IConfig _config;
		private static ILogger _logger;
		private static IComponent[] _decorators;

		protected AdSnapshot()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_cancellationToken = _cancellationTokenSource.Token;
		}

		public static AdSnapshot Instance()
		{
			_instance = _instance ?? new AdSnapshot();
			//_adNotifyCollection = _adNotifyCollection ?? new AdNotifyCollection();
			_connected = null;
			return _instance;
		}

		private static void AdInit()
		{
			if ( _connected == null )
			{
				try
				{
					_directoryEntry = _directoryEntry ?? new DirectoryEntry("LDAP://" + _config.ServerAddress, _config.UserName, _config.UserPassword);
					_directorySearcher = _directorySearcher ?? new DirectorySearcher(_directoryEntry)
					{
						Filter = "(|(isDeleted=true)(&(objectClass=user)(objectCategory=person))(&(objectClass=computer)(objectCategory=computer))(&(objectClass=group)(objectCategory=group)))",
						SearchScope = SearchScope.Subtree,
						ExtendedDN = ExtendedDN.Standard,
						Tombstone = true
					};
					_directorySearcher.PropertiesToLoad.AddRange(new[] { "lastknownparent", "whencreated", "whenchanged" });

					_directorySearcher.DirectorySynchronization = new DirectorySynchronization(DirectorySynchronizationOptions.ObjectSecurity);

					foreach ( SearchResult res in _directorySearcher.FindAll() )
					{ }

					_connected = true;
				} catch
				{
					_connected = false;
					_logger.Log("Some error occured on Active Directory connecting", OutputTarget.Console);
				}
			}
		}

		private static void InitializeSnapshot()
		{
			if ((bool)!_connected) return;
			_cookie = _directorySearcher.DirectorySynchronization.GetDirectorySynchronizationCookie();
		}

		private static void CompareSnapshot()
		{
			var dirSync = new DirectorySynchronization(DirectorySynchronizationOptions.ObjectSecurity, _cookie);

			_directorySearcher = new DirectorySearcher(_directoryEntry)
			{
				DirectorySynchronization = dirSync
			};

			foreach ( SearchResult res in _directorySearcher.FindAll() )
			{
				var delta = res.Properties;
				var found = false;
				foreach ( string prop in delta.PropertyNames )
				{
					switch ( prop.ToLower() )
					{
						case "objectguid":
						case "adspath":
						case "instancetype":
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
						case "physicaldeliveryofficename":
						case "sn":
						case "telephonenumber":
						case "title":
						case "userworkstations":
						case "distinguishedname":
						if ( delta.Contains("isdeleted") && bool.TryParse(delta["isdeleted"][0].ToString(), out bool deleted) && deleted )
						{
							ProcessDeleted(prop, delta);
							found = true;
						} else
						{
							ProcessChanged(res.GetDirectoryEntry(), prop, delta);
							found = true;
						}
						break;
					}
				}
				if ( found ) OnAdChanged?.Invoke();
				_directorySearcher.DirectorySynchronization.ResetDirectorySynchronizationCookie(_cookie);
				InitializeSnapshot();
			}
		}

		private static void ProcessDeleted(string prop, ResultPropertyCollection delta)
		{
			
		}

		private static void ProcessChanged(DirectoryEntry directoryEntry, string prop, ResultPropertyCollection delta)
		{
			var schemeClass = directoryEntry.SchemaClassName ?? string.Empty;
			
			if (!schemeClass.Equals("computer", StringComparison.OrdinalIgnoreCase)
			    && !schemeClass.Equals("user", StringComparison.OrdinalIgnoreCase)
			    && !schemeClass.Equals("group", StringComparison.OrdinalIgnoreCase))
				return;

			foreach ( object val in delta[prop] )
			{
				var name = directoryEntry?.Name?.Substring(3) ?? string.Empty;
				var parent = directoryEntry?.Parent.Name ?? string.Empty;
				if ( name.Equals(val.ToString(), StringComparison.OrdinalIgnoreCase) )
					continue;
				//_adNotifyCollection.Push(CreateNotifyMessage(schemeClass, name, prop, val, parent));
			}
		}

		private static AdNotifyMessage CreateNotifyMessage(string schemeClass, string name, string property, object value, string parent)
		{
			string val;
			if ( value is string )
				val = value.ToString();
			else if ( value is byte[] )
				val = parent;
			else
				val = value.ToString();

			// TODO !!! доделать
			if ( schemeClass.Equals("computer", StringComparison.OrdinalIgnoreCase) )
				return new AdNotifyMessageUserModified(schemeClass, name, property, val);
			else
				return new AdNotifyMessageUserModified(schemeClass, name, property, val);
		}

		public async void RunAsync(int loopPeriodInMilliseconds)
		{
			AdInit();
			InitializeSnapshot();
			_enabled = true;

			await Task.Run(() =>
			{
				while ( _enabled )
				{
					if ( _cancellationToken.IsCancellationRequested )
					{
						_enabled = false;
						break;
					}
					CompareSnapshot();
					Task.Delay(loopPeriodInMilliseconds, _cancellationToken);
				}
			}, _cancellationToken);
		}

		public void Stop()
		{
			_cancellationTokenSource.Cancel();
			_connected = null;
		}

		public override void Init(params IComponent[] decorators)
		{
			base.Init(decorators);

			HelpMsg.HelpMsg.MsgList.Add("/NotificationsOn [/non] - Subscribe to notifications on AD changes");
			HelpMsg.HelpMsg.MsgList.Add("/NotificationsOff [/noff] - Unsubscribe to notifications on AD changes");

			_decorators = decorators;
			_logger = _decorators?.OfType<ILogger>().FirstOrDefault();
			_config = (Config.Config)_decorators?.OfType<IConfig>().FirstOrDefault();
			_logger?.Log("Initializing Service: Active Directory Snapshot...", OutputTarget.Console);

			if (string.IsNullOrEmpty(_config?.ServerAddress) || string.IsNullOrEmpty(_config?.UserName) ||
			    string.IsNullOrEmpty(_config?.UserPassword)) return;

			Config.Config.OnConfigUpdated += Config_OnConfigUpdated;
			RunAsync(3000);
			//AdNotifySender.Instance(this, _config);
		}

		private void Config_OnConfigUpdated(IConfig config)
		{
			_config = config;

			if ( string.IsNullOrEmpty(_config?.ServerAddress) || string.IsNullOrEmpty(_config?.UserName) ||
			     string.IsNullOrEmpty(_config?.UserPassword) ) return;

			Stop();
			Thread.Sleep(1000);
			RunAsync(3000);
			//AdNotifySender.Instance(this, _config);
		}
	}
}
