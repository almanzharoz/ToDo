using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Expo3.AdminApp.Projections
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Event>, IGetProjection, ISearchProjection, IInsertProjection, IUpdateProjection, IWithName, IWithOwner
	{
		public string Name { get; set; }
		public string Caption { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime FinishDateTime { get; set; }
		public Address Address { get; set; }
		public EEventType Type { get; set; }
		[JsonProperty]
		public BaseUserProjection Owner { get; private set; }

		public EventProjection() { }

		public EventProjection(BaseUserProjection owner) // for new event
		{
			Owner = owner;
		}
	}

	public class EventRemoveProjection : BaseEntityWithVersion, IProjection<Event>, IRemoveProjection, IGetProjection
	{
	}
}