using System;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Category : BaseEntityWithVersion, IModel, IProjection<Category>, IGetProjection, IInsertProjection, IUpdateProjection, ISearchProjection
	{
        [Keyword]
		public Category Top { get; set; }
		public string Name { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}