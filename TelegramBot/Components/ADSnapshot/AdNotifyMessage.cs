namespace AlexAd.ActiveDirectoryTelegramBot.Bot.Components.ADSnapshot
{
	/// <summary>
	///		Абстрактный класс для формировани объекта-оповещения об изменениях в Active Directory
	/// </summary>
	internal abstract class AdNotifyMessage
	{
		private AdNotifyMessage()
		{
		}

		/// <summary>
		///		Формироване объекта-оповещения об изменениях в Active Directory
		/// </summary>
		/// <param name="changingObject">Какой именно объект изменился (пользователь, компьютер, группа)</param>
		/// <param name="changingOperation">Тип изменения (изменение, удаление, добавление)</param>
		/// <param name="schemeClass">Тип изменившегомя объекта (пользователь, компьютер, группа)</param>
		/// <param name="name">Имя изменившегося объекта</param>
		/// <param name="property">Изменившееся поле</param>
		/// <param name="value">Изменившееся значение</param>
		protected AdNotifyMessage(AdNotifyType.ChangingObjectType changingObject,
			AdNotifyType.ChangingOperationType changingOperation, string schemeClass, string name, string property,
			string value)
		{
			Object = changingObject;
			Operation = changingOperation;
			SchemeClass = schemeClass;
			Name = name;
			Property = property;
			Value = value;
		}

		// TODO v2 ChangingObjectType, ChangingOperationType Заготовки для будущего использования
		private AdNotifyType.ChangingObjectType Object { get; }
		private AdNotifyType.ChangingOperationType Operation { get; }
		public string SchemeClass { get; }
		public string Name { get; }
		public string Property { get; set; }
		public string Value { get; set; }
	}
}