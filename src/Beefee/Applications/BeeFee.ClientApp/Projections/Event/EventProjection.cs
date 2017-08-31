using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventProjection : BaseEntity, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Url { get; private set; }

		[JsonProperty]
		public string Name { get; private set; }

		[Keyword]
		[JsonProperty]
		public BaseCategoryProjection Category { get; private set; }

		[JsonProperty]
		public EEventType Type { get; private set; }

		[JsonProperty]
		public TicketPrice[] Prices { get; private set; }

		[JsonProperty]
		public EventPage Page { get; private set; }
	}
}