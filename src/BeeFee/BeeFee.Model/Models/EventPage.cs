using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Nest;
using Newtonsoft.Json;

namespace BeeFee.Model
{
	/// <summary>
	/// Настройки мероприятия, отображаемые посетителю
	/// </summary>
	public struct EventPage
	{
		/// <summary>
		/// Внутренний HTML страницы мероприятия
		/// </summary>
		[JsonProperty]
		public string Html { get; private set; }
		/// <summary>
		/// Заголовок title страницы
		/// </summary>
		[JsonProperty]
		public string Title { get; private set; }
		[JsonProperty]
		public string Caption { get; private set; }
		[JsonProperty]
		public string Cover { get; private set; }
		/// <summary>
		/// Галерея
		/// </summary>
		[JsonProperty]
		[Keyword(Index = false)]
		public string[] Images { get; private set; }

		[JsonProperty]
		public Address Address { get; private set; }
		[JsonProperty]
		public string Date { get; private set; }
		[JsonProperty]
		public string Category { get; private set; }
		[JsonProperty]
		public string Company { get; private set; }

		public EventPage SetHtml(string html)
		{
			Html = html;
			return this;
		}

		public EventPage(string caption, string category, string cover, EventDateTime date, Address address, string html)
		{
			Title = caption;
			Caption = caption;
			Category = category;
			Cover = cover;
			Address = address;
			Date = date.ToString();
			Html = html;

			Company = null;
			Images = null;
		}
	}
}