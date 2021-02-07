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


namespace AlexAd.ActiveDirectoryTelegramBot.Bot.ADSnapshot
{
	public class AdSnapshot
	{
		public delegate void AdChanged();
		public static event AdChanged OnAdChanged;

		private static DirectoryEntry _directoryEntry;
		private static DirectorySearcher _directorySearcher;
		private static AdNotifyCollection _adNotifyCollection;
		private static byte[] _cookie;
		private static AdSnapshot _instance;
		private static bool _enabled;
		private static Config.Config _config;

		protected AdSnapshot() { }

		public static AdSnapshot Instance(Config.Config config)
		{
			_instance = _instance ?? new AdSnapshot();
			_config = _config ?? config;
			_adNotifyCollection = _adNotifyCollection ?? new AdNotifyCollection();
			return _instance;
		}

		private static void AdInit()
		{
			_directoryEntry = _directoryEntry ?? new DirectoryEntry("LDAP://" + _config.ServerAddress, _config.UserName, _config.UserPassword);
			_directorySearcher = _directorySearcher ?? new DirectorySearcher(_directoryEntry)
			{
				// TODO OU вынести в конфиг
				Filter = "(|(&(objectClass=user)(objectCategory=person))(&(objectClass=computer)(objectCategory=computer))(&(objectClass=group)(objectCategory=group)))",
				//Filter = "(|(isDeleted=TRUE)(&(objectClass=user))(&(objectClass=computer))(&(objectClass=group)))",
				//Filter = "(|(isDeleted=TRUE)(&(objectClass=user))(&(objectClass=computer))(&(objectClass=group)))",
				//Filter = "isDeleted=true",
				SearchScope = SearchScope.OneLevel,
				ExtendedDN = ExtendedDN.Standard,
				Tombstone = true
			};
			_directorySearcher.PropertiesToLoad.AddRange(new[] { "lastknownparent", "whencreated", "whenchanged" });
		}

		private static void InitializeSnapshot()
		{
			AdInit();
			_directorySearcher.DirectorySynchronization = new DirectorySynchronization(DirectorySynchronizationOptions.ObjectSecurity);

			foreach ( SearchResult res in _directorySearcher.FindAll() )
			{ }

			_cookie = _directorySearcher.DirectorySynchronization.GetDirectorySynchronizationCookie();
		}

		private static void CompareSnapshot()
		{
			// TODO вывести ошибку чтения кук
			if ( _cookie == null )
				return;

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
					// TODO список полей запихнуть в конфиг???
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

			foreach ( object val in delta[prop] )
			{
				var name = directoryEntry?.Name?.Substring(3) ?? string.Empty;
				var parent = directoryEntry?.Parent.Name ?? string.Empty;
				if ( name.Equals(val.ToString(), StringComparison.OrdinalIgnoreCase) )
					continue;
				_adNotifyCollection.Push(CreateNotifyMessage(schemeClass, name, prop, val, parent));
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

			if ( schemeClass.Equals("computer", StringComparison.OrdinalIgnoreCase) )
				return new AdNotifyMessageUserModified(schemeClass, name, property, val);
			else
				return new AdNotifyMessageUserModified(schemeClass, name, property, val);
		}



		public async void RunAsync(int loopPeriodInMilliseconds)
		{
			InitializeSnapshot();
			_enabled = true;

			await Task.Run(() =>
			{
				while ( _enabled )
				{
					CompareSnapshot();
					//Task.Delay(loopPeriodInMilliseconds);
				}
			});
		}

		public void Stop()
		{
			_enabled = false;
		}
	}
}
