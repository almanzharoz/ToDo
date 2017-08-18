using System;
using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Nest;

namespace Expo3.Model.Models
{
	/// <summary>
	/// Модель мероприятия для фильтрации и отображения ячеек
	/// </summary>
	public abstract class Event : BaseEntityWithVersion, IModel, IProjection, IWithName
	{
		public Category Category { get; set; }
		public string Name { get; set; }
		[Keyword]
		public User Owner { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		public EEventType Type { get; set; }
		public TicketPrice[] Prices { get; set; }
		[Nested]
		public EventPage Page { get; set; }
	}
}