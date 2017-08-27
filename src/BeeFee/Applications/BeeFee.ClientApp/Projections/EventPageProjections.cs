using Core.ElasticSearch.Domain;
using BeeFee.Model;
using Newtonsoft.Json;

namespace BeeFee.ClientApp.Projections
{
	public class EventPageProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection
	{
		[JsonProperty]
		public EventPage Page { get; private set; }
	}
}