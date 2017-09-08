using BeeFee.Model.Interfaces;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventCellProjection : BaseEntity, IProjection<Model.Models.Event>, IWithUrl, ISearchProjection
	{
		public EventPageCell Page { get; }

		public string Url { get; }

		public EventCellProjection(string id, string url, EventPageCell page) : base(id)
		{
			Page = page;
			Url = url;
		}

	}
}