using System;
using System.Linq;
using Core.ElasticSearch.Domain;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Projections
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

		internal EventProjection Change(string name, string url, EventDateTime dateTime, Address address, EEventType type,
			BaseCategoryProjection category, TicketPrice[] prices, string html)
		{
			Name = name;
			Url = url.IfNull(name, CommonHelper.UriTranslit);
			DateTime = dateTime;
			Address = address;
			Type = type;
			Category = category;
			Prices = prices;
			Page = Page.SetHtml(html);
			return this;
		}
	}

	internal class NewEvent : BaseNewEntity, IProjection<Event>, IWithName, IWithOwner
	{
		public string Name { get; }
		public string Url { get; }

		public EventDateTime DateTime { get; }

		public EEventType Type { get; }

		public Address Address { get; }

		public EventPage Page { get; }

		[Keyword]
		public BaseCategoryProjection Category { get; }

		[Keyword]
		public BaseUserProjection Owner { get; }

		public NewEvent() { }

		private readonly ThrowCollection _throws = new ThrowCollection();

		public NewEvent(BaseUserProjection owner, BaseCategoryProjection category, string name, string url, EEventType type, EventDateTime dateTime, Address address, string html)
		{
			Owner = owner.HasNotNullEntity(_throws, nameof(owner));
			Category = category.HasNotNullEntity(_throws, nameof(category));
			Name = name.HasNotNullArg(_throws, nameof(name));
			Url = url.IfNull(name, CommonHelper.UriTranslit);
			DateTime = dateTime;
			Type = type;
			Address = address;
			Page = new EventPage(name, category.Name, null, dateTime, address, html);
			_throws.Throw();
		}
	}
}