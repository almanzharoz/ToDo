using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace BeeFee.AdminApp.Projections.Event
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection, IRemoveProjection, IUpdateProjection, IWithName, IWithOwner
	{
		[Keyword]
		[JsonProperty]
		public BaseCategoryProjection Category { get; private set; }
		[JsonProperty]
		public string Name { get; private set; }
		[JsonProperty]
		public EventDateTime DateTime { get; private set; }
		[JsonProperty]
		public Address Address { get; private set; }
		[JsonProperty]
		public EEventType Type { get; private set; }

		[Keyword]
		[JsonProperty]
		public BaseUserProjection Owner { get; private set; }

		internal EventProjection ChangeCategory(BaseCategoryProjection newCategory)
		{
			Category = newCategory;
			return this;
		}
	}
}