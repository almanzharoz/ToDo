using BeeFee.Model.Interfaces;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventCellProjection : BaseEntity, IProjection<Model.Models.Event>, IWithUrl, ISearchProjection
	{
		[JsonProperty]
		public EventPageCell Page { get; private set; }

		[JsonProperty]
		public string Url { get; private set; }
	}
}