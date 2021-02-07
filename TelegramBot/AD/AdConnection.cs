﻿using System;
using System.DirectoryServices.AccountManagement;
using AlexAd.ActiveDirectoryTelegramBot.Bot.Logger;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.AD
{
    public class AdConnection
    {
        private static AdConnection _instance;
        private static Logger.Logger _logger;
        private static Config.Config _config;

        protected AdConnection() { }
        
        public static AdConnection Instance(Logger.Logger logger, Config.Config config)
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
	                _logger.Log($"Active Directory Connecting Error. Check Identity Params.", OutputTarget.Console & OutputTarget.File);
	                return false;
                }
            }
            catch (Exception e)
            {
	            principalContext = null;
                _logger.Log($"Active Directory Initializing Error: {e.Message}", OutputTarget.Console & OutputTarget.File);
                return false;
            }
            
            return true;
        }

        private PrincipalContext GetPrincipalContext() =>
            new PrincipalContext(ContextType.Domain, _config.ServerAddress, _config.UserName, _config.UserPassword);
        
    }
}