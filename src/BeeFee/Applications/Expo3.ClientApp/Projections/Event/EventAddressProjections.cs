using Core.ElasticSearch.Domain;
using Expo3.Model.Embed;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections.Event
{
	public class EventAddressProjections : BaseEntityWithVersion, IProjection<Model.Models.Event>, ISearchProjection
	{
		[JsonProperty]
		public Address Address { get; private set; }
	}
}