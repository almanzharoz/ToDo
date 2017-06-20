using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Core.Tests.Models;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace Core.Tests
{
	public class TestRepository : BaseRepository<ElasticSettings>
	{
		public TestRepository(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container) : base(loggerFactory, settings, mapping, container)
		{
		}

		public Projections.Category GetCategory(string id) => Get<Category, Projections.Category>(id);

		public Category AddCategory(string name, string parent) =>
			new Category() { Name = name, Top = GetCategory(parent.HasNotNullArg("parentId")).HasNotNullArg("parent") }.Fluent(Insert);

		public Category AddCategory(string name) =>
			new Category() { Name = name }.Fluent(Insert);
	}
}