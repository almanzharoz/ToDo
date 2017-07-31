using Core.ElasticSearch.Domain;

namespace Expo3.Model
{
	public class EventPage : BaseEntityWithVersion, IModel, IProjection
	{
		public string Html { get; set; }
	}
}