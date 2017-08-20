﻿using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Nest;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections.Event
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Name { get; private set; }

		[Keyword]
		[JsonProperty]
		public BaseCategoryProjection Category { get; private set; }


		[JsonProperty]
		public EventDateTime DateTime { get; private set; }

		[JsonProperty]
		public Address Address { get; private set; }

		[JsonProperty]
		public EEventType Type { get; private set; }

		[JsonProperty]
		public TicketPrice[] Prices { get; private set; }
	}

	public class EventSearchProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Name { get; private set; }

        [JsonProperty]
        public EventDateTime DateTime { get; private set; }
    }
}