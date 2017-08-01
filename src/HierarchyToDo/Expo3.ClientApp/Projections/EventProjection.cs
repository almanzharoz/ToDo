using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public string Caption { get; private set; }

		[JsonProperty]
		public string Cover { get; private set; }

		[JsonProperty]
		public DateTime StartDateTime { get; private set; }

		[JsonProperty]
		public DateTime FinishDateTime { get; private set; }

		[JsonProperty]
		public Address Address { get; private set; }

		[JsonProperty]
		public EEventType Type { get; private set; }

		[JsonProperty]
		public TicketPrice[] Prices { get; private set; }
	}

	public class EventProjectionWithVisitors : BaseEntityWithVersion, IProjection<Event>, IGetProjection, IUpdateProjection
	{
		public Visitor[] Visitors { get; set; }
	}

	public class EventSearchProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public string Caption { get; private set; }

		[JsonProperty]
		public string Cover { get; private set; }
	}
}