using BeeFee.Model.Interfaces;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventCellProjection : BaseEntity, IProjection<Model.Models.Event>, IWithUrl, ISearchProjection
	{
		public EventPageCell Page { get; private set; }

		public string Url { get; private set; }
	}
}