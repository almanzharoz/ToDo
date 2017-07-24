using System;

namespace Core.ElasticSearch
{
	/// <summary>
	/// Базовый класс настройки подключения к БД
	/// </summary>
	public abstract class BaseElasticSettings
	{
		public Uri Url { get; }

		protected BaseElasticSettings(Uri url)
		{
			Url = url;
		}
	}
}