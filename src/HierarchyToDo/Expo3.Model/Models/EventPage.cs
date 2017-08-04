using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Nest;

namespace Expo3.Model
{
	/// <summary>
	/// Настройки мероприятия, отображаемые посетителю
	/// </summary>
	public class EventPage : BaseEntityWithVersion, IModel, IProjection
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
		/// <summary>
		/// Галерея
		/// </summary>
		[Keyword(Index = false)]
		public string[] Images { get; set; }
	}
}