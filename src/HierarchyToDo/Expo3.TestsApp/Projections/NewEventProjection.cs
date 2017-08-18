using Core.ElasticSearch.Domain;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Nest;

namespace Expo3.TestsApp.Projections
{
	internal class NewEventProjection : BaseEntity, IProjection<Event>, IInsertProjection
	{
		[Keyword]
		public BaseCategoryProjection Category { get; set; }
		public string Name { get; set; }
		[Keyword]
		public BaseUserProjection Owner { get; set; }
		public EventDateTime DateTime { get; set; }
		public Address Address { get; set; }
		public EEventType Type { get; set; }
		public TicketPrice[] Prices { get; set; }
		public EventPage Page { get; set; }

		public NewEventProjection() { }
		public NewEventProjection(BaseUserProjection owner) // for new event
		{
			Owner = owner;
		}

	}
}