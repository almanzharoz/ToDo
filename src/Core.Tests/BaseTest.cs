using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Core.Tests.Models;
using Core.Tests.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
    [TestClass]
    public abstract class BaseTest
    {
        protected TestRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddElastic<ElasticSettings>()
                    .AddRepository<TestRepository, ElasticSettings>()
                .BuildServiceProvider();

            serviceProvider.UseElasticForTests<ElasticSettings>(map => map

                .AddMapping<Producer>()
                .AddMapping<Category>()
                .AddMapping<Product>()

                .AddProjection<Producer, Producer>()
                .AddProjection<Category, Category>()
                .AddProjection<CategoryProjection, Category>()
                .AddProjection<ProductProjection, Product, Category, Category>()
                .AddProjection<Product, Product, Category, Category>());

            _repository = serviceProvider.GetService<TestRepository>();
        }
    }
}