using System;
using System.Collections.Generic;
using System.Linq;
using Expo3.ClientApp.Services;
using Expo3.Model;
using Expo3.Model.Embed;
using Expo3.Model.Models;
using Expo3.TestsApp;
using Expo3.TestsApp.Projections;
using Expo3.TestsApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Expo3.ClientApp.Tests
{
    [TestClass]
    public class CategoryServiceTests
    {

        private CategoryService _categoryService;
        private TestsCaterogyService _testsCaterogyService;

        [TestInitialize]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddExpo3Model(new Uri("http://localhost:9200/"))
                .AddExpo3ClientApp()
                .AddExpo3TestsApp()
                .AddSingleton(new UserName("1"))
                .AddLogging()
                .BuildServiceProvider();

            serviceProvider
                .UseExpo3Model(true)
                .UseExpo3TestsApp()
                .UseExpo3ClientApp();

            _categoryService = serviceProvider.GetService<CategoryService>();
            _testsCaterogyService = serviceProvider.GetService<TestsCaterogyService>();

        }

        [TestMethod]
        public void SearchCategories()
        {
            _testsCaterogyService.AddCategory("Category One");
            _testsCaterogyService.AddCategory("Category Two");

            var categories = _categoryService.SearchCategories().ToList();

            Assert.AreEqual(categories.Count, 2);

            Assert.IsTrue(categories.Any(c => c.Name.Equals("Category One", StringComparison.OrdinalIgnoreCase)));

            categories = _categoryService.SearchCategories("Category One").ToList();

            Assert.AreEqual(categories.Count, 1);

            Assert.IsTrue(categories.Any(c => c.Name.Equals("Category One", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
