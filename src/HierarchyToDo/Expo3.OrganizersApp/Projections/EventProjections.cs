using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Expo3.Model.Models;
using Nest;
using Newtonsoft.Json;

namespace Expo3.OrganizersApp.Projections
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, ISearchProjection,
		IInsertProjection, IUpdateProjection, IWithName, IWithOwner
	{
		public EventProjection()
		{
		}

		public EventProjection(BaseUserProjection owner) // for new event
		{
			Owner = owner;
		}

		[Keyword]
		public Category Category { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		public EEventType Type { get; set; }
		public string Name { get; set; }
		public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }

		[Keyword]
		[JsonProperty]
		public BaseUserProjection Owner { get; private set; }
	}

	public class EventRemoveProjection : BaseEntityWithVersion, IProjection<Event>, IRemoveProjection, IGetProjection
	{
	}
}