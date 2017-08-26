using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Helpers;
using Expo3.Model.Models;
using Nest;

namespace Expo3.TestsApp.Projections
{
	internal class NewEvent : BaseNewEntity, IProjection<Event>
	{
		[Keyword]
		public BaseCategoryProjection Category { get; set; }
		[Keyword]
		public string Url { get; set; }
		public string Name { get; set; }
		[Keyword]
		public BaseUserProjection Owner { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		public EEventType Type { get; set; }
		public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }

		public NewEvent() { }
		public NewEvent(BaseUserProjection owner, string name, string url=null) // for new event
		{
			Owner = owner;
			Name = name.Trim();
			Url = (url ?? CommonHelper.UriTranslit(name)).ToLowerInvariant();
		}

	}
}