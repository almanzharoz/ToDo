using System.Collections.Generic;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Models;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Expo3.AdminApp.Services
{
	public class CategoryService : BaseExpo3Service
	{
		public CategoryService(ILoggerFactory loggerFactory, Expo3ElasticConnection settings,
			ElasticScopeFactory<Expo3ElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory, user)
		{
		}

		public bool Add(string name)
			=> InsertWithVersion(new CategoryProjection {Name = name});

		public bool Remove(string id)
			=> Remove(Get<CategoryProjection>(id));

		public bool Rename(string id, int version, string name)
			=> Update<CategoryProjection>(id, version, x =>
			{
				x.Name = name;
				return x;
			});

		public IReadOnlyCollection<CategoryProjection> GetAllCategories()
			=> Search<Category, CategoryProjection>(q => q.MatchAll());
	}
}