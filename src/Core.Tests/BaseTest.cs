﻿using Core.ElasticSearch;
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
        protected TestService _repository;

        [TestInitialize]
        public void Setup()
        {
	        var serviceProvider = new ServiceCollection()
		        .AddLogging()
		        .AddElastic(new ElasticConnection(), s => s
			        .AddService<TestService>())
		        .BuildServiceProvider();

	        serviceProvider.UseElastic<ElasticConnection>(map => map

		        .AddMapping<Producer>(x => x.SecondIndex)
		        .AddMapping<Category>(x => x.FirstIndex)
		        .AddMapping<Product>(x => x.FirstIndex)
		        .AddMapping<User>(x => x.FirstIndex), s => s

		        .AddProjection<Producer, Producer>()
		        .AddProjection<NewProducer, Producer>()
		        .AddProjection<Category, Category>()
		        .AddProjection<NewCategory, Category>()
		        .AddProjection<CategoryProjection, Category>()
		        .AddProjection<ProductProjection, Product, Category>()
		        .AddProjection<Product, Product, Category>()
		        .AddProjection<NewProduct, Product, Category>()
		        .AddProjection<User, User>()
		        .AddProjection<NewUser, User>()
		        .AddProjection<UserUpdateProjection, User>(), true);

            _repository = serviceProvider.GetService<TestService>();
        }
    }
}