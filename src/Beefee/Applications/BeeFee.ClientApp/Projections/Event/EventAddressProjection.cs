using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventAddressProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, ISearchProjection
	{
		public Address Address { get; private set; }
	}
}