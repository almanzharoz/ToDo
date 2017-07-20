using System;

namespace Core.ElasticSearch
{
	/// <summary>
	/// Базовый класс настройки подключения к БД
	/// </summary>
	public abstract class BaseElasticSettings
	{
		public Uri Url { get; }
		public string IndexName { get; }

		protected BaseElasticSettings(Uri url, string indexName)
		{
			Url = url;
			IndexName = indexName;
		}
	}
}