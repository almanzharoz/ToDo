using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;

namespace BeeFee.AdminApp.Projections.Event
{
	public class EventProjection : BaseEntityWithVersion, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection, IRemoveProjection, IUpdateProjection, IWithName, IWithOwner
	{
		[Keyword]
		public BaseCategoryProjection Category { get; private set; }
		public string Name { get; private set; }
		public EventDateTime DateTime { get; private set; }
		public Address Address { get; private set; }
		public EEventType Type { get; private set; }

		[Keyword]
		public BaseUserProjection Owner { get; private set; }

		internal EventProjection ChangeCategory(BaseCategoryProjection newCategory)
		{
			Category = newCategory;
			return this;
		}
	}
}