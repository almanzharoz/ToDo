using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Nest;

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