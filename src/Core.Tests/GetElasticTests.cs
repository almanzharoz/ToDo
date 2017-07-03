using System;
using System.Linq;
using Core.ElasticSearch;
using Core.Tests.Models;
using Core.Tests.Projections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using SharpFuncExt;

namespace Core.Tests
{
    [TestClass]
    public class GetElasticTests : BaseTest
    {
        [TestMethod]
        public void AddObjectWithoutParentAndRelated()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);

            Assert.IsNotNull(category.Id);
        }

        [TestMethod]
        public void AddObjectWithInvalidRelatedAndWithoutParent()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            var childCategory = new Category() { Name = "Child Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            Assert.ThrowsException<Exception>(() => { _repository.Insert(childCategory, true); });
        }

        [TestMethod]
        public void AddObjectWithValidRelatedAndWithoutParent()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(parentCategory, true);
            var childCategory = new Category() { Name = "Child Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory, true);
            Assert.IsNotNull(childCategory.Id);
        }

        [TestMethod]
        public void AddObjectWithInvalidParentAndWithoutRelated()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            var product = new Product() { Name = "Product", Parent = category };
            Assert.ThrowsException<Exception>(() => { _repository.Insert(product, true); });
        }

        [TestMethod]
        public void AddObjectWithValidParentAndWithoutRelated()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            var product = new Product() { Name = "Product", Parent = category, FullName = new FullName() { Name = "Product", Category = category.Name } };
            _repository.Insert(product, true);
            Assert.IsNotNull(product.Id);
        }

        [TestMethod]
        public void GetObjectByIdWithoutAutoLoadAndWithoutParent()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            var category = new Category() { Name = "Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };

            _repository.Insert(parentCategory, true);
            _repository.Insert(category, true);

            var loadCategory = _repository.Get<Category, Category>(category.Id, false);

            Assert.IsNotNull(loadCategory);
            Assert.IsNull(loadCategory.Top);
            Assert.AreNotEqual(loadCategory, category);
            Assert.AreEqual(loadCategory.Name, category.Name);
            Assert.AreEqual(loadCategory.Id, category.Id);
        }

        [TestMethod]
        public void GetObjectByIdWithoutAutoLoadAndWithParent()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            var product = new Product() { Name = "Product", Parent = category, FullName = new FullName() { Name = "Product", Category = category.Name } };
            _repository.Insert(category, true);
            _repository.Insert(product, true);

            var loadProduct = _repository.Get<Product, Product>(product.Id, false);

            Assert.IsNotNull(loadProduct);
            Assert.IsNull(loadProduct.Parent);
            Assert.IsNotNull(loadProduct.FullName);
            Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
        }

        [TestMethod]
        public void GetObjectByIdWithAutoLoadAndWithoutParent()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            var category = new Category() { Name = "Category", Top = parentCategory };

            _repository.Insert(parentCategory, true);
            _repository.Insert(category, true);

            var loadCategory = _repository.Get<Category, Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.IsNotNull(loadCategory.Top);
            Assert.AreNotEqual(loadCategory, category);
            Assert.AreEqual(loadCategory.Name, category.Name);
            Assert.AreEqual(loadCategory.Id, category.Id);
        }

        [TestMethod]
        public void GetObjectByIdWithAutoLoadAndWithParent()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            var product = new Product() { Name = "Product", Parent = category, FullName = new FullName() { Name = "Product", Category = category.Name } };

            _repository.Insert(category, true);
            _repository.Insert(product, true);

            var loadProduct = _repository.Get<Product, Product, Category, Category>(product.Id, category.Id, true);

            Assert.IsNotNull(loadProduct);
            Assert.IsNotNull(loadProduct.Parent);
            Assert.IsNotNull(loadProduct.FullName);
            Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
        }

        [TestMethod]
        public void UpdateObjectSimply()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            category.Name = "New Category";
            _repository.Update(category, true);

            var loadCategory = _repository.Get<Category, Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.AreNotEqual(loadCategory.Version, category.Version);
            Assert.AreEqual(loadCategory.Name, "New Category");
        }

        [TestMethod]
        public void UpdateObjectSimplyWithInvalidObjectId()
        {
            var category = new Category() { Name = "Category" };
            category.Name = "New Category";
            category.Id = "NewId";
            Assert.ThrowsException<Exception>(() => _repository.Update(category, true));
        }

        [TestMethod]
        public void UpdateObjectSimplyWithAnotherVersion()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            category.Name = "New1 Category";
            _repository.Update(category, true);
            category.Version--;
            category.Name = "New2 Category";
            Assert.ThrowsException<Exception>(() => _repository.Update(category, true));

            var loadCategory = _repository.Get<Category, Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.AreEqual(loadCategory.Name, "New1 Category");
        }

        [TestMethod]
        public void UpdateObjectByQueryWithInvalidQuery()
        {
            Assert.ThrowsException<Exception>(() => _repository.Update(Query<Category>.Ids(x => x.Values("NewId")), new UpdateByQueryBuilder<Category>().Set(x => x.Name, "New Category"), true));
        }

        [TestMethod]
        public void UpdateObjectByQueryFuncSet()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            _repository.Update(Query<Category>.Ids(x => x.Values(category.Id)), new UpdateByQueryBuilder<Category>().Set(x => x.Name, "New Category"), true);

            var loadCategory = _repository.Get<Category, Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.AreEqual(loadCategory.Name, "New Category");
        }

        [TestMethod]
        public void UpdateObjectByQueryFuncUnset()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            _repository.Update(Query<Category>.Ids(x => x.Values(category.Id)), new UpdateByQueryBuilder<Category>().Unset(x => x.Name), true);

            var loadCategory = _repository.Get<Category, Category>(category.Id, true);

            Assert.IsNotNull(loadCategory);
            Assert.IsNull(loadCategory.Name);
        }

        [TestMethod]
        public void RemoveObject()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            _repository.Remove(category);
            var loadCategory = _repository.Get<Category, Category>(category.Id, true);
            Assert.IsNull(loadCategory);
        }

        [TestMethod]
        public void RemoveObjectByInvelidId()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            category.Id = "NewId";
            Assert.ThrowsException<Exception>(() => _repository.Remove(category));
        }

        [TestMethod]
        public void RemoveObjectByQuery()
        {
            var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category, true);
            _repository.Remove<Category>(Query<Category>.Ids(x => x.Values(category.Id)));
            var loadCategory = _repository.Get<Category, Category>(category.Id, true);
            Assert.IsNull(loadCategory);
        }

        [TestMethod]
        public void RemoveObjectByQueryWithInvalidQuery()
        {
            Assert.ThrowsException<Exception>(() => _repository.Remove<Category>(Query<Category>.Ids(x => x.Values("NewId"))));
        }

        [TestMethod]
        public void SearchSimpleCategory()
        {
            var category1 = new Category() { Name = "Test Category1", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category1, true);
            var category2 = new Category() { Name = "Test Category2", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category2, true);
            var category3 = new Category() { Name = "Test Category3", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category3, true);
            var category4 = new Category() { Name = "Test Category4", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category4, true);
            var category5 = new Category() { Name = "Test Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category5, true);

            var categories = _repository.Search<Category, Category>(Query<Category>.Match(x => x.Field(c => c.CreatedOnUtc).Query("category")));

            Assert.AreEqual(categories.Count(c => c.Name.Equals("Test Category")), 1);
        }

        [TestMethod]
        public void SearchCategoryByRelatedWithoutLimitationAndWithoutLoad()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(parentCategory, true);
            var childCategory1 = new Category() { Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parentCategory };
            _repository.Insert(childCategory1, true);
            var childCategory2 = new Category() { Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parentCategory };
            _repository.Insert(childCategory2, true);
            var childCategory3 = new Category() { Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parentCategory };
            _repository.Insert(childCategory3, true);
            var category1 = new Category() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category1, true);
            var category2 = new Category() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category2, true);

            var childCategories = _repository.Search<Category, Category>(Query<Category>.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 0, 0, false);

            Assert.AreEqual(childCategories.Count, 3);
            Assert.IsFalse(childCategories.Any(c => c.Name.Equals("Category1")));
            Assert.IsFalse(childCategories.Any(c => c.Name.Equals("Category2")));
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
            Assert.AreEqual(childCategories.FirstOrDefault().Name, "Child Category3");
            Assert.IsNull(childCategories.FirstOrDefault().Top);
        }

        [TestMethod]
        public void SearchCategoryByRelatedWithLimitationAndWithoutLoad()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(parentCategory, true);
            var childCategory1 = new Category() { Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory1, true);
            var childCategory2 = new Category() { Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory2, true);
            var childCategory3 = new Category() { Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory3, true);
            var category1 = new Category() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category1, true);
            var category2 = new Category() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category2, true);

            var childCategories = _repository.Search<Category, Category>(Query<Category>.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Ascending(c => c.CreatedOnUtc), 1, 1, false);

            Assert.AreEqual(childCategories.Count, 1);
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
            Assert.IsNull(childCategories.FirstOrDefault().Top);
        }

        [TestMethod]
        public void SearchCategoryByRelatedWithLimitationAndWithLoad()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(parentCategory, true);
            var childCategory1 = new Category() { Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory1, true);
            var childCategory2 = new Category() { Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory2, true);
            var childCategory3 = new Category() { Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory3, true);
            var category1 = new Category() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category1, true);
            var category2 = new Category() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category2, true);

            var childCategories = _repository.Search<Category, Category>(Query<Category>.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Ascending(c => c.CreatedOnUtc), 1, 1, true);

            Assert.AreEqual(childCategories.Count, 1);
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
            Assert.IsNotNull(childCategories.FirstOrDefault().Top);
        }

        [TestMethod]
        public void SearchCategoryByParentWithPagingAndWithoutLoad()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(parentCategory, true);
            var childCategory1 = new Category() { Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory1, true);
            var childCategory2 = new Category() { Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory2, true);
            var childCategory3 = new Category() { Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory3, true);
            var category1 = new Category() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category1, true);
            var category2 = new Category() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category2, true);

            var childCategories = _repository.Search<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 1, 2, false);

            Assert.AreEqual(childCategories.Count, 1);
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
            Assert.IsNull(childCategories.FirstOrDefault().Top);
        }

        [TestMethod]
        public void SearchCategoryByParentWithPagingAndWithLoad()
        {
            var parentCategory = new Category() { Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(parentCategory, true);
            var childCategory1 = new Category() { Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory1, true);
            var childCategory2 = new Category() { Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory2, true);
            var childCategory3 = new Category() { Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(childCategory3, true);
            var category1 = new Category() { Name = "Category1", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category1, true);
            var category2 = new Category() { Name = "Category2", CreatedOnUtc = DateTime.UtcNow };
            _repository.Insert(category2, true);

            var childCategories = _repository.Search<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 1, 2, true);

            Assert.AreEqual(childCategories.Count, 1);
            Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
            Assert.IsNotNull(childCategories.FirstOrDefault().Top);
        }
    }
}
