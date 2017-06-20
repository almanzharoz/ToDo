using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
    [TestClass]
    public class GetElasticTests : BaseTest
    {
        [TestMethod]
        public void AddSimpleCategory()
        {
	        var category = _repository.AddCategory("Test Category");

			Assert.IsNotNull(category);
			Assert.IsNotNull(category.Id);
			Assert.AreEqual(category.Name, "Test Category");

	        var loadCategory = _repository.GetCategory(category.Id);

			Assert.IsNotNull(loadCategory);
			Assert.AreNotEqual(loadCategory, category);
			Assert.AreEqual(loadCategory.Name, category.Name);
			Assert.AreEqual(loadCategory.Id, category.Id);
        }
    }
}
