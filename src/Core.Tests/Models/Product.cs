﻿using Core.ElasticSearch.Domain;
using Core.Tests.Projections;
using Nest;
using Newtonsoft.Json;

namespace Core.Tests.Models
{
	public class Product : BaseEntityWithParentAndVersion<Models.Category>, IModel, IProjection<Product>, IGetProjection, IInsertProjection, ISearchProjection
    {
		public string Name { get; set; }
		[Keyword]
		public Producer Producer { get; set; }
		public Projections.FullName FullName { get; set; }

	    [Completion]
	    public string Title { get; set; }
	}
}