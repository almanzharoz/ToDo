using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Projections.Event
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection,
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
			Page = Page.SetHtml(html).Change(name, category.Name, null, dateTime, address);
			return this;
		}
	}
}