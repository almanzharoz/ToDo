using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventProjection : BaseEntity, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection
	{
		public string Url { get; private set; }

		public string Name { get; private set; }

		[Keyword]
		public BaseCategoryProjection Category { get; private set; }

		public EEventType Type { get; private set; }

		public TicketPrice[] Prices { get; private set; }

		public EventPage Page { get; private set; }
	}
}