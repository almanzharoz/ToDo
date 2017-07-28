using System;
using Core.ElasticSearch.Domain;
using Expo3.Model;
using Nest;
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
        public Price Price { get; private set; }
        [JsonProperty]
        public DateTime StartDateTime { get; private set; }
        [JsonProperty]
        public DateTime FinishDateTime { get; private set; }
        [JsonProperty]
        public Address Address { get; private set; }
    }
}