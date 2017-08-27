using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Expo3.Model.Models;
using Nest;
using Newtonsoft.Json;

namespace Expo3.OrganizerApp.Projections
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, ISearchProjection,
		IUpdateProjection, IRemoveProjection, IWithName, IWithOwner
	{
		[JsonProperty]
		[Keyword]
		public BaseCategoryProjection Category { get; private set; }
		[JsonProperty]
		public EventDateTime DateTime { get; private set; }
		[JsonProperty]
		public Address Address { get; private set; }
		[JsonProperty]
		public EEventType Type { get; private set; }
		[JsonProperty]
		public string Url { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public TicketPrice[] Prices { get; private set; }
		[JsonProperty]
		public EventPage Page { get; private set; }

		[Keyword]
		[JsonProperty]
		public BaseUserProjection Owner { get; private set; }

		internal EventProjection Change(string name, EventDateTime dateTime, Address address, EEventType type,
			BaseCategoryProjection category, TicketPrice[] prices, string html)
		{
			Name = name;
			DateTime = dateTime;
			Address = address;
			Type = type;
			Category = category;
			Prices = prices;
			Page = Page.SetHtml(html);
			return this;
		}
	}

	public class NewEvent : BaseNewEntity, IProjection<Event>, IWithName, IWithOwner
	{
		public string Name { get; set; }

		[Keyword]
		[JsonProperty]
		public BaseUserProjection Owner { get; private set; }

		public NewEvent() { }

		public NewEvent(BaseUserProjection owner)
		{
			Owner = owner;
		}
	}
}