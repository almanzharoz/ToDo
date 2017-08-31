using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections.Event
{
	public class EventAddressProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, ISearchProjection
	{
		[JsonProperty]
		public Address Address { get; private set; }
	}
}