﻿using Core.ElasticSearch.Domain;
using Nest;

namespace Core.ElasticSearch.Tests.Models
{
	public class Product : BaseEntityWithParentAndVersion<Models.Category>, IModel, IProjection<Product>, IGetProjection,  ISearchProjection
    {
		public string Name { get; set; }
		[Keyword]
		public Producer Producer { get; set; }
		public Projections.FullName FullName { get; set; }

	    [Completion]
	    public string Title { get; set; }

	    public Product(string id, Category parent) : base(id, parent)
	    {
	    }

	    public Product(string id, Category parent, int version) : base(id, parent, version)
	    {
	    }
    }

	public class NewProduct : BaseNewEntityWithParent<Models.Category>, IProjection<Product>
	{
		public string Name { get; set; }
		[Keyword]
		public Producer Producer { get; set; }
		public Projections.FullName FullName { get; set; }

		[Completion]
		public string Title { get; set; }

		public NewProduct(Category parent) : base(parent)
		{
		}
	}
}