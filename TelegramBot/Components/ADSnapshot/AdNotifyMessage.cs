namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Класс для формировани объекта-оповещения об изменениях в Active Directory
	/// </summary>
	internal class AdNotifyMessage
	{
		/// <summary>
		///		Формироване объекта-оповещения об изменениях в Active Directory
		/// </summary>
		/// <param name="schemeClass">Тип изменившегомя объекта (пользователь, компьютер, группа)</param>
		/// <param name="name">Имя изменившегося объекта</param>
		/// <param name="property">Изменившееся поле</param>
		/// <param name="value">Изменившееся значение</param>
		public AdNotifyMessage(string schemeClass, string name, string property, string value)
		{
			SchemeClass = schemeClass;
			Name = name;
			Property = property;
			Value = value;
		}

		public string SchemeClass { get; }
		public string Name { get; }
		public string Property { get; set; }
		public string Value { get; set; }
	}
}