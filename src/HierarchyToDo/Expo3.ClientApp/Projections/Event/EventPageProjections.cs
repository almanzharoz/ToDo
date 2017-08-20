using Core.ElasticSearch.Domain;
using Expo3.Model;
using Newtonsoft.Json;

namespace Expo3.ClientApp.Projections.Event
{
	public class EventPageProjections : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection
	{
		[JsonProperty]
		public EventPage Page { get; private set; }
	}
}