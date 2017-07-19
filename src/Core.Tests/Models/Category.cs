using System;
using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Category : BaseEntity, IProjection<Category>, IWithVersion
    {
        [Keyword]
		public Category Top { get; set; }
		public string Name { get; set; }
        [JsonIgnore]
        public int Version { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}