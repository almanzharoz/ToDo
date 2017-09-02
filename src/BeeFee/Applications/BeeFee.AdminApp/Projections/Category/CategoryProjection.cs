﻿using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.AdminApp.Projections.Category
{
	public class CategoryProjection : BaseEntityWithVersion, IProjection<Model.Models.Category>, IRemoveProjection,
		IGetProjection, IUpdateProjection, ISearchProjection
	{
		public string Url { get; private set; }
		public string Name { get; private set; }

		internal CategoryProjection Rename(string name, string url)
		{
			Url = url.IfNull(name, CommonHelper.UriTranslit);
			Name = name;
			return this;
		}
	}
}