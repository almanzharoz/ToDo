using System;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Category : BaseEntityWithVersion, IModel, IProjection<Category>, IGetProjection, IUpdateProjection, ISearchProjection, IRemoveProjection
	{
        [Keyword]
		public Category Top { get; set; }
		public string Name { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

	public class NewCategory : BaseNewEntity, IProjection<Category>
	{
		[Keyword]
		public Category Top { get; set; }
		public string Name { get; set; }
		public DateTime CreatedOnUtc { get; set; }

		public NewCategory() : base() { }
		public NewCategory(string id) : base(id) { }
	}
}