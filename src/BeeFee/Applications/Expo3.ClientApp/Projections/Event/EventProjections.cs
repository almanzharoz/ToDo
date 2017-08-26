using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Nest;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections.Event
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

	public class EventCellProjection : BaseEntity, IProjection<Model.Models.Event>, ISearchProjection
	{
		[JsonProperty]
		public EventPageCell Page { get; private set; }
    }

	public struct EventPageCell
	{
		public string Url { get; set; }
		public string Caption { get; set; }
		public string Cover { get; set; }

		public string Date { get; set; }
		public string Category { get; set; }
	}
}