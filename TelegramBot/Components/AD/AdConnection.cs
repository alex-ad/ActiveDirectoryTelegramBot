using System;
using System.DirectoryServices.AccountManagement;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Config;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Components.Logger;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.AD
{
	internal class AdConnection
    {
        private static AdConnection _instance;
        private static ILogger _logger;
        private static IConfig _config;

        private AdConnection() { }
        
        public static AdConnection Instance(ILogger logger, IConfig config)
        {
	        _logger = logger;
	        _config = config;
			_instance = _instance ?? new AdConnection();
	        return _instance;
        }

		public bool TryConnect(out PrincipalContext principalContext)
		{
            try
            {
                principalContext = GetPrincipalContext();
                if (principalContext == null ||
                    !principalContext.ValidateCredentials(_config.UserName, _config.UserPassword))
                {
	                _logger.Log($"Active Directory Connecting Error. Check Identity Params.", OutputTarget.Console | OutputTarget.File);
	                return false;
                }
            }
            catch (Exception e)
            {
	            principalContext = null;
                _logger.Log($"Active Directory Initializing Error: {e.Message}", OutputTarget.Console | OutputTarget.File);
                return false;
            }
            
            return true;
        }

        private PrincipalContext GetPrincipalContext() =>
            new PrincipalContext(ContextType.Domain, _config.ServerAddress, _config.UserName, _config.UserPassword);
        
    }
}