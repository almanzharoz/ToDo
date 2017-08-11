using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Nest;

namespace Expo3.Model
{
	/// <summary>
	/// Настройки мероприятия, отображаемые посетителю
	/// </summary>
	public struct EventPage
	{
		[Keyword]
		public string UrlName { get; set; }
		/// <summary>
		/// Внутренний HTML страницы мероприятия
		/// </summary>
		public string Html { get; set; }
		/// <summary>
		/// Заголовок title страницы
		/// </summary>
		public string Title { get; set; }
		public string Caption { get; set; }
		public string Cover { get; set; }
		/// <summary>
		/// Галерея
		/// </summary>
		[Keyword(Index = false)]
		public string[] Images { get; set; }

		public Address Address { get; set; }
		public string Date { get; set; }
		public string Category { get; set; }
		public string Company { get; set; }
	}
}