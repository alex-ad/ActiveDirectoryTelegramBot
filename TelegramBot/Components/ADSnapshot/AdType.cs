using System;

namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Значения битовых масок и значений, формируемых при получении данных из Active Directory
	/// </summary>
	internal static class AdType
	{
		/// <summary>
		///		Расшифровка значения родительской группы
		/// </summary>
		public enum PrimaryGroupId
		{
			DomainUsers = 513,
			DomainComputers = 515,
			DomainControllers = 516
		}

		/// <summary>
		///		Расшифровка значения типа аккаунта
		/// </summary>
		[Flags]
		public enum SamAccountType
		{
			DomainObject = 0x0,
			GroupObject = 0x10000000,
			NonSecurityGroupObject = 0x10000001,
			AliasObject = 0x20000000,
			NonSecurityAliasObject = 0x20000001,
			UserObject = 0x30000000,
			MachineAccount = 0x30000001,
			TrustAccount = 0x30000002,
			AppBasicAccount = 0x40000000,
			AppQueryGroup = 0x40000001,
			AccountTypeMax = 0x7fffffff
		}

		/// <summary>
		///		Расшифровка значения характеристик аккаунта
		/// </summary>
		[Flags]
		public enum UserAccountControl
		{
			Script = 1,
			AccountDisabled = 2,
			HomeDirRequired = 8,
			Lockout = 16,
			PasswordNotRequired = 32,
			PasswordCantChanged = 64,
			EncryptedTextPasswordAllowed = 128,
			TempDuplicateAccount = 256,
			NormalAccount = 512,
			InterDomainTrustAccount = 2048,
			WorkStationTrustAccount = 4096,
			ServerTrustAccount = 8192,
			DontExpirePassword = 65536,
			MnsLogonAccount = 131072,
			SmartCardRequired = 262144,
			TrustedForDelegation = 524288,
			NotDelegated = 1048576,
			UseDesKeyOnly = 2097152,
			DontRequiredPreAuth = 4194304,
			PasswordExpired = 8388608,
			TrustedToAuthForDelegation = 16777216,
			PartialSecretsAccount = 67108864
		}
	}
}