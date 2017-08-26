using System.Collections.Generic;
using Core.ElasticSearch;
using Expo3.AdminApp.Projections;
using Expo3.Model;
using Expo3.Model.Helpers;
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

		public bool Add(string name, string url=null)
			=> Insert(new NewCategory(url, name.Trim()), true);

		public bool Remove(string id) => Remove<CategoryProjection>(id, true);

		public bool Rename(string id, int version, string name, string url)
			=> Update<CategoryProjection>(id, version, x => x.Rename(name, url), true);

		public IReadOnlyCollection<CategoryProjection> GetAllCategories()
			=> Filter<Category, CategoryProjection>(null, s => s.Ascending(p => p.Name));
	}
}