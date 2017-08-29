using Core.ElasticSearch.Domain;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Nest;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections
{
	public class EventProjection : BaseEntity, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		[JsonProperty]
		public string Url{ get; private set; }

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

	public class EventCellProjection : BaseEntity, IProjection<Model.Models.Event>, IWithUrl, ISearchProjection
	{
		[JsonProperty]
		public EventPageCell Page { get; private set; }

		[JsonProperty]
		public string Url { get; private set; }
	}

	public struct EventPageCell
	{
		public string Caption { get; set; }
		public string Cover { get; set; }

		public string Date { get; set; }
		public string Category { get; set; }
	}
}