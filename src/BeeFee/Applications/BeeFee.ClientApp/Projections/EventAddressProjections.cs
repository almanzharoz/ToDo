using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections
{
	public class EventAddressProjections : BaseEntityWithVersion, IProjection<Model.Models.Event>, ISearchProjection
	{
		[JsonProperty]
		public Address Address { get; private set; }
	}
}