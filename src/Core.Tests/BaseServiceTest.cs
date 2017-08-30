using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Core.ElasticQueryBuilder.Commands;
using Core.ElasticSearch;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Exceptions;
using Core.Tests.Models;
using Core.Tests.Projections;
using Elasticsearch.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Newtonsoft.Json;
using SharpFuncExt;

namespace Core.Tests
{
	[TestClass]
	public class BaseServiceTest : BaseTest
	{
		[TestMethod]
		public void AddObjectWithoutParentAndRelated()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);

			Assert.IsNotNull(category.Id);
		}

		[TestMethod]
		public void AddObjectWithInvalidRelatedAndWithoutParent()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			var childCategory = new Category() {Name = "Child Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			Assert.ThrowsException<UnexpectedElasticsearchClientException>(() =>
			{
				_repository.Insert(childCategory, true);
			});
		}

		[TestMethod]
		public void AddObjectWithValidRelatedAndWithoutParent()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory = new Category() {Name = "Child Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory, true);
			Assert.IsNotNull(childCategory.Id);
		}

		[TestMethod]
		public void AddObjectWithInvalidParentAndWithoutRelated()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			var product = new Product() {Name = "Product", Parent = category};
			Assert.ThrowsException<QueryException>(() => _repository.Insert<Product, Category>(product, true));
		}

		[TestMethod]
		public void AddObjectWithValidParentAndWithoutRelated()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);
			var product = new Product()
			{
				Name = "Product",
				Parent = category,
				FullName = new FullName() {Name = "Product", Category = category.Name}
			};
			_repository.Insert<Product, Category>(product, true);
			Assert.IsNotNull(product.Id);
			Assert.IsNotNull(product.Parent);
			Assert.AreEqual(product.Parent.Id, category.Id);
			Assert.AreEqual(product.Parent.Name, category.Name);
		}

		[TestMethod]
		public void GetObjectByIdWithoutAutoLoadAndWithoutParent()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			var category = new Category() {Name = "Category", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};

			_repository.Insert(parentCategory, true);
			_repository.Insert(category, true);

			var loadCategory = _repository.Get<Category>(category.Id, false);

			Assert.IsNotNull(loadCategory);
			Assert.IsNotNull(loadCategory.Top);
			Assert.AreEqual(loadCategory.Top.Id, parentCategory.Id);
			Assert.IsNull(loadCategory.Top.Name);
			Assert.AreNotEqual(loadCategory, category);
			Assert.AreEqual(loadCategory.Name, category.Name);
			Assert.AreEqual(loadCategory.Id, category.Id);
		}

		[TestMethod]
		public void GetProjectionObjectByIdWithoutAutoLoadAndWithoutParent()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};

			_repository.Insert(category, true);

			var loadCategory = _repository.Get<CategoryProjection>(category.Id, false);

			Assert.IsNotNull(loadCategory);
			Assert.IsNull(loadCategory.Top);
			Assert.AreNotEqual(loadCategory, category);
			Assert.AreEqual(loadCategory.Name, category.Name);
			Assert.AreEqual(loadCategory.Id, category.Id);
		}

		[TestMethod]
		public void GetObjectByIdWithoutAutoLoadAndWithParent()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			var product = new Product()
			{
				Name = "Product",
				Parent = category,
				FullName = new FullName() {Name = "Product", Category = category.Name}
			};
			_repository.Insert(category, true);
			_repository.Insert<Product, Category>(product, true);

			var loadProduct = _repository.Get<Product, Category>(product.Id, category.Id, false);

			Assert.IsNotNull(loadProduct);
			Assert.IsNotNull(loadProduct.Parent);
			Assert.AreEqual(loadProduct.Parent.Id, category.Id);
			Assert.IsNull(loadProduct.Parent.Name);
			Assert.IsNotNull(loadProduct.FullName);
			Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
		}

		[TestMethod]
		public void GetProjectionObjectByIdWithoutAutoLoadAndWithParent()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			var product = new Product()
			{
				Name = "Product",
				Parent = category,
				FullName = new FullName() {Name = "Product", Category = category.Name}
			};
			_repository.Insert(category, true);
			_repository.Insert<Product, Category>(product, true);

			var loadProduct = _repository.Get<ProductProjection, Category>(product.Id, category.Id, false);

			Assert.IsNotNull(loadProduct);
			Assert.IsNotNull(loadProduct.Parent);
			Assert.AreEqual(loadProduct.Parent.Id, category.Id);
			Assert.IsNull(loadProduct.Parent.Name);
			Assert.IsNotNull(loadProduct.FullName);
			Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
		}

		[TestMethod]
		public void GetObjectByIdWithAutoLoadAndWithoutParent()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			var category = new Category() {Name = "Category", Top = parentCategory};

			_repository.Insert(parentCategory, true);
			_repository.Insert(category, true);

			var loadCategory = _repository.Get<Category>(category.Id, true);

			Assert.IsNotNull(loadCategory);
			Assert.IsNotNull(loadCategory.Top);
			Assert.AreEqual(loadCategory.Top.Id, parentCategory.Id);
			Assert.AreEqual(loadCategory.Top.Name, parentCategory.Name);
			Assert.AreNotEqual(loadCategory, category);
			Assert.AreEqual(loadCategory.Name, category.Name);
			Assert.AreEqual(loadCategory.Id, category.Id);
		}

		[TestMethod]
		public void GetProjectionObjectByIdWithAutoLoadAndWithoutParent()
		{
			var category = new Category() {Name = "Category"};

			_repository.Insert(category, true);

			var loadCategory = _repository.Get<CategoryProjection>(category.Id, true);

			Assert.IsNotNull(loadCategory);
			Assert.IsNull(loadCategory.Top);
			Assert.AreNotEqual(loadCategory, category);
			Assert.AreEqual(loadCategory.Name, category.Name);
			Assert.AreEqual(loadCategory.Id, category.Id);
		}

		[TestMethod]
		public void GetObjectByIdWithAutoLoadAndWithParent()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			var product = new Product()
			{
				Name = "Product",
				Parent = category,
				FullName = new FullName() {Name = "Product", Category = category.Name}
			};

			_repository.Insert(category, true);
			_repository.Insert<Product, Category>(product, true);

			var loadProduct = _repository.Get<Product, Category>(product.Id, category.Id, true);

			Assert.IsNotNull(loadProduct);
			Assert.IsNotNull(loadProduct.Parent);
			Assert.IsNotNull(loadProduct.FullName);
			Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
			Assert.AreEqual(loadProduct.Parent.Id, product.Parent.Id);
			Assert.AreEqual(loadProduct.Parent.Name, product.Parent.Name);
		}

		[TestMethod]
		public void GetProjectionObjectByIdWithAutoLoadAndWithParent()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			var product = new Product()
			{
				Name = "Product",
				Parent = category,
				FullName = new FullName() {Name = "Product", Category = category.Name}
			};

			_repository.Insert(category, true);
			_repository.Insert<Product, Category>(product, true);

			var loadProduct = _repository.Get<ProductProjection, Category>(product.Id, category.Id, true);

			Assert.IsNotNull(loadProduct);
			Assert.IsNotNull(loadProduct.Parent);
			Assert.IsNotNull(loadProduct.FullName);
			Assert.AreEqual(loadProduct.FullName.Name, product.FullName.Name);
		}

		[TestMethod]
		public void UpdateObjectSimply()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);
			category.Name = "New Category";
			_repository.Update(category, true);

			var loadCategory = _repository.Get<Category>(category.Id, true);

			Assert.IsNotNull(loadCategory);
			Assert.AreEqual(loadCategory.Version, category.Version);
			Assert.AreEqual(loadCategory.Name, "New Category");
		}

		//[TestMethod]
		//public void UpdateObjectSimplyWithInvalidObjectId()
		//{
		//    var category = new Category() { Name = "Category" };
		//    category.Name = "New Category";
		//    category.Id = "NewId";
		// category.Version = 2;
		//    Assert.ThrowsException<QueryException>(() => _repository.Update(category, true));
		//}

		// Тест больше не актуален, т.к. нельзя никак получить проекции и управлять номером версии в ней. А так же нельзя никак обновить проекцию в базе созданную из вне движка.
		//[TestMethod]
		//      public void UpdateObjectSimplyWithAnotherVersion()
		//      {
		//          var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
		//          _repository.Insert(category, true);
		//          category.Name = "New1 Category";
		//          _repository.Update(category, true);
		//          category.Version--;
		//          category.Name = "New2 Category";
		//          Assert.ThrowsException<VersionException>(() => _repository.Update(category, true));

		//          var loadCategory = _repository.Get<Category>(category.Id, true);

		//          Assert.IsNotNull(loadCategory);
		//          Assert.AreEqual(loadCategory.Name, "New1 Category");
		//      }

		[TestMethod]
		public void UpdateObjectByQueryFuncSet()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);
			_repository.Update<Category>(q => q.Ids(x => x.Values(category.Id)), u => u.Set(x => x.Name, "New Category"), true);

			var loadCategory = _repository.Get<Category>(category.Id, true);

			Assert.IsNotNull(loadCategory);
			Assert.AreEqual(loadCategory.Name, "New Category");
		}

		[TestMethod]
		public void UpdateObjectByQueryFuncUnset()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);
			_repository.Update<Category>(q => q.Ids(x => x.Values(category.Id)), u => u.Unset(x => x.Name), true);

			var loadCategory = _repository.Get<Category>(category.Id, true);

			Assert.IsNotNull(loadCategory);
			Assert.IsNull(loadCategory.Name);
		}

		[TestMethod]
		public void RemoveObject()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);
			_repository.Remove(category);
			var loadCategory = _repository.Get<Category>(category.Id, true);
			Assert.IsNull(loadCategory);
		}

		// Ограничил такое поведение на уровне компиляции (Id - internal set)
		//[TestMethod]
		//public void RemoveObjectByInvalidId()
		//{
		//    var category = new Category() { Name = "Category", CreatedOnUtc = DateTime.UtcNow };
		//    category.Id = "NewId";
		// category.Version = 1;
		//    Assert.ThrowsException<VersionException>(() => _repository.Remove(category));
		//}

		[TestMethod]
		public void RemoveObjectByQuery()
		{
			var category = new Category() {Name = "Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category, true);
			_repository.Remove<Category>(q => q.Ids(x => x.Values(category.Id)));
			var loadCategory = _repository.Get<Category>(category.Id, true);
			Assert.IsNull(loadCategory);
		}

		[TestMethod]
		public void SearchSimpleCategory()
		{
			var category1 = new Category() {Name = "Test Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Test Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);
			var category3 = new Category() {Name = "Test Category3", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category3, true);
			var category4 = new Category() {Name = "Test Category4", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category4, true);
			var category5 = new Category() {Name = "Test Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category5, true);

			var categories = _repository.Filter<Category, Category>(q => q.Match(x => x.Field(c => c.Name).Query("category")));

			Assert.AreEqual(categories.Count, 1);
			Assert.IsTrue(categories.Any(c => c.Name.Equals("Test Category")));
		}

		[TestMethod]
		public void SearchSimpleCategoryProjection()
		{
			var category1 = new Category() {Name = "Test Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Test Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);
			var category3 = new Category() {Name = "Test Category3", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category3, true);
			var category4 = new Category() {Name = "Test Category4", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category4, true);
			var category5 = new Category() {Name = "Test Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category5, true);

			var categories =
				_repository.Filter<Category, CategoryProjection>(q => q.Match(x => x.Field(c => c.Name).Query("category")));

			Assert.AreEqual(categories.Count, 1);
			Assert.IsTrue(categories.Any(c => c.Name.Equals("Test Category")));
		}

		[TestMethod]
		public void SearchCategoryByRelatedWithoutLimitationAndWithoutLoad()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory1 = new Category() {Name = "Child Category1", CreatedOnUtc = DateTime.UtcNow, Top = parentCategory};
			_repository.Insert(childCategory1, true);
			var childCategory2 = new Category() {Name = "Child Category2", CreatedOnUtc = DateTime.UtcNow, Top = parentCategory};
			_repository.Insert(childCategory2, true);
			var childCategory3 = new Category() {Name = "Child Category3", CreatedOnUtc = DateTime.UtcNow, Top = parentCategory};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			var childCategories = _repository.Filter<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 0, 0,
				false);

			Assert.AreEqual(childCategories.Count, 3);
			Assert.IsFalse(childCategories.Any(c => c.Name.Equals("Category1")));
			Assert.IsFalse(childCategories.Any(c => c.Name.Equals("Category2")));
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
			Assert.AreEqual(childCategories.FirstOrDefault().Name, "Child Category3");
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByRelatedWithLimitationAndWithoutLoad()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory1 = new Category() {Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory1, true);
			var childCategory2 = new Category() {Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory2, true);
			var childCategory3 = new Category() {Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			var childCategories = _repository.Filter<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Ascending(c => c.CreatedOnUtc), 1, 1,
				false);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Id, parentCategory.Id);
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByRelatedWithLimitationAndWithLoad()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory1 = new Category() {Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory1, true);
			var childCategory2 = new Category() {Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory2, true);
			var childCategory3 = new Category() {Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			var childCategories = _repository.Filter<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Ascending(c => c.CreatedOnUtc), 1, 1,
				true);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category2")));
			Assert.IsNotNull(childCategories.FirstOrDefault().Top);
		}

		[TestMethod]
		public void SearchCategoryByParentWithLambdaAndWithoutLoad()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory1 = new Category() {Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory1, true);
			var childCategory2 = new Category() {Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory2, true);
			var childCategory3 = new Category() {Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			var childCategories = _repository.Search<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 1, 2,
				false);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
			Assert.AreEqual(childCategories.FirstOrDefault().Top.Id, parentCategory.Id);
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByParentWithLambdaAndWithLoad()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory1 = new Category() {Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory1, true);
			var childCategory2 = new Category() {Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory2, true);
			var childCategory3 = new Category() {Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			var childCategories = _repository.Search<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), sort => sort.Descending(c => c.CreatedOnUtc), 1, 2,
				true);

			Assert.AreEqual(childCategories.Count, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category1")));
			Assert.IsNotNull(childCategories.FirstOrDefault().Top);
		}


		[TestMethod]
		public void SearchCategoryByParentWithPagingAndWithoutLoad()
		{
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(parentCategory, true);
			var childCategory1 = new Category() {Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory1, true);
			var childCategory2 = new Category() {Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory2, true);
			var childCategory3 = new Category() {Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			var childCategories = _repository.FilterPager<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 0, 1, sort => sort.Descending(c => c.CreatedOnUtc),
				false);

			Assert.AreEqual(childCategories.Total, 3);
			Assert.AreEqual(childCategories.Limit, 1);
			Assert.AreEqual(childCategories.Page, 1);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
			Assert.IsNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void SearchCategoryByParentWithPagingAndWithLoad()
		{
			var sw = new Stopwatch();
			var parentCategory = new Category() {Name = "Parent Category", CreatedOnUtc = DateTime.UtcNow};
			sw.Restart();
			_repository.Insert(parentCategory, true);
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			var childCategory1 = new Category() {Name = "Child Category1", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			sw.Restart();
			_repository.Insert(childCategory1, true);
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			var childCategory2 = new Category() {Name = "Child Category2", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			sw.Restart();
			//_repository.Insert(childCategory2, true);
			_repository.GetNestClient().Index(childCategory2, s => s.Index("first_test_index").Index("category").Refresh(Refresh.False));
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			var childCategory3 = new Category() {Name = "Child Category3", Top = parentCategory, CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(childCategory3, true);
			var category1 = new Category() {Name = "Category1", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category1, true);
			var category2 = new Category() {Name = "Category2", CreatedOnUtc = DateTime.UtcNow};
			_repository.Insert(category2, true);

			//var childCategories = _repository.FilterPager<Category, Category>(q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 1, 2, sort => sort.Descending(c => c.CreatedOnUtc), true);
			sw.Restart();
			var childCategories = _repository.FilterPager<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 1, 2, sort => sort.Descending(c => c.CreatedOnUtc),
				true);
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);

			_repository.Clear();

			sw.Restart();
			childCategories = _repository.FilterPager<Category, Category>(
				q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 1, 2, sort => sort.Descending(c => c.CreatedOnUtc),
				true);
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);

			//sw.Restart();
			//for (var i = 0; i < 10000; i++)
			//{
			//	childCategories = _repository.FilterPager<Category, Category>(
			//		q => q.Match(c => c.Field(f => f.Top).Query(parentCategory.Id)), 1, 2, sort => sort.Descending(c => c.CreatedOnUtc),
			//		true);
			//}
			//sw.Stop();
			//Console.WriteLine(sw.ElapsedMilliseconds);

			Assert.AreEqual(childCategories.Total, 3);
			Assert.AreEqual(childCategories.Limit, 2);
			Assert.AreEqual(childCategories.Page, 1);
			Assert.AreEqual(childCategories.Count, 2);
			Assert.IsTrue(childCategories.Any(c => c.Name.Equals("Child Category3")));
			Assert.IsNotNull(childCategories.FirstOrDefault().Top);
			Assert.IsNotNull(childCategories.FirstOrDefault().Top.Name);
		}

		[TestMethod]
		public void InsertIntoAnotherIndex()
		{
			var producer = new Producer {Name = "Producer1"};
			_repository.Insert(producer, true);

			var loaded = _repository.Get<Producer>(producer.Id);

			Assert.AreEqual(loaded.Name, producer.Name);
		}

		[TestMethod]
		public void InsertIntoAnotherIndexWithJoin()
		{
			var producer = new Producer {Name = "Producer1"};
			_repository.Insert(producer, true);

			var category = new Category {Name = "Category1"};
			_repository.Insert(category, true);

			var product = new Product {Name = "Product1", Producer = producer, Parent = category};
			_repository.Insert<Product, Category>(product, true);

			var loaded = _repository.Get<Product, Category>(product.Id, category.Id, true);

			Assert.AreEqual(loaded.Name, product.Name);
			Assert.AreEqual(loaded.Producer.Id, product.Producer.Id);
			Assert.AreEqual(loaded.Producer.Name, product.Producer.Name);
			Assert.AreEqual(loaded.Parent.Id, product.Parent.Id);
			Assert.AreEqual(loaded.Parent.Name, product.Parent.Name);
		}

		[TestMethod]
		public void UpdateProjection()
		{
			// 1. Добавить новую проекцию с 1 private set полем и несколькими с public set
			// 2. Вставить в базу полную проекцию включая поля, которые не указаны в новой проекции
			// 3. Достать по Id новую проекцию (у проекции для этого должен быть IGetProjection)
			// 4. Обновить
			// 5. Достать полную проекцию и проверить, что обновляемые поля обновлены, а другие остались нетронутыми

			var user = new User {Login = "user1", Email = "user1@user1.ru", Password = "123", Salt = "111"};
			_repository.Insert(user, true);

			var loaded = _repository.Get<UserUpdateProjection>(user.Id, true);
			loaded.Email = "new@user1.ru";
			loaded.Password = "newPass";

			_repository.Update(loaded, true);

			var loadedFullUser = _repository.Get<User>(user.Id, true);

			Assert.AreEqual("user1", loadedFullUser.Login);
			Assert.AreEqual("new@user1.ru", loadedFullUser.Email);
			Assert.AreEqual("newPass", loadedFullUser.Password);
			Assert.AreEqual("111", loadedFullUser.Salt);
		}

		[TestMethod]
		public void HtmlStripTest()
		{

		}

		[TestMethod]
		public void AutocompleteTest()
		{
			// Есть уже анализатор, но лучше покурить это: https://www.red-gate.com/simple-talk/dotnet/net-development/how-to-build-a-search-page-with-elasticsearch-and-net/
		}

		[TestMethod]
		public void NestQueryTest()
		{
			var sw = new Stopwatch();
			//var q = Nest.Query<Product>.Bool(b => b.Must(m => m.Term(p => p.Name, "123")));
			//var s = _repository.GetNestClient().Serializer.SerializeToString(q);
			//sw.Start();
			//for (var i = 0; i < 10000; i++)
			//{
			//	q = Nest.Query<Product>.Bool(b => b.Must(m => m.Term(p => p.Name, Guid.NewGuid().ToString())));
			//	s = _repository.GetNestClient().Serializer.SerializeToString(q);
			//}
			//Console.WriteLine(s);
			//sw.Stop();
			//Console.WriteLine(sw.ElapsedMilliseconds);
			//Console.WriteLine(sw.ElapsedTicks);

			sw.Restart();
			var sd = new IndexDescriptor<Category>(new Category(){Name = "name"}).Type("category").Index("first_test_index");
			//var sd = new SearchDescriptor<object>().Type(Types.Parse("project")).Query(q => q.Bool(b => b.Filter(Nest.Query<object>.Ids(z => z.Values(new[] { "AVx9SuIapC_PV5uTcKSj" })))));
			_repository.GetNestClient().Serializer.SerializeToBytes(sd);
			sw.Stop();
			Console.WriteLine(sw.ElapsedTicks);
			var d = new PostData<object>(_repository.GetNestClient()
				.Serializer.SerializeToBytes(new Category() {Name = Guid.NewGuid().ToString()}));
			var a = new List<Task<IIndexResponse>>(100);
			sw.Restart();
			for (var i = 0; i < 100; i++)
			{
				//sd = new SearchDescriptor<object>().Type(Types.Parse("project"))
				//	.Query(q => q.Bool(b => b.Filter(Nest.Query<object>.Ids(z => z.Values(new[] {Guid.NewGuid().ToString()})))));
				//sd = new IndexDescriptor<Category>(new Category() { Name = Guid.NewGuid().ToString() }).Type("category").Index("first_test_index");
				//_repository.GetNestClient().Serializer.SerializeToString(sd);
				var r = _repository.GetNestClient().IndexAsync(new IndexRequest<Category>(new Category() { Name = Guid.NewGuid().ToString() }, "first_test_index", "category"));
				a.Add(r);
				//_repository.GetClient().IndexAsync<Category>("first_test_index", "category", d);
			}
			Task.WaitAll(a.ToArray());
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(sw.ElapsedTicks);
		}

		[TestMethod]
		public void QueryTest()
		{
			Stopwatch sw1 = new Stopwatch();
			Stopwatch sw2 = new Stopwatch();
			sw1.Start();
			var q = new Query1<Product>();
			q.Bool(x => x.Must("Name", 1));
			q.Build();
			sw1.Stop();
			string s;
			sw2.Start();
			for (var i = 0; i < 10000; i++)
			{
				//q = new Query<Product>();
				//q.Bool(x => x.Must("Name", 1));
				//q.Build();
				//s = q.GetJson();
				s = q.Bool(x => x.Must("Name", 1)).GetJson();
			}
			sw2.Stop();
			Console.WriteLine(sw2.ElapsedMilliseconds);
		}

		[TestMethod]
		public void QueryTest2()
		{
			var sw = new Stopwatch();
			sw.Start();
			var q1 = QueryFactory.GetOrAdd<Product>(q => q.Bool(x => x.Filter(y => y.Term(p => p.Name, "123"))));
			sw.Stop();
			Console.WriteLine(sw.ElapsedTicks);
			Console.WriteLine(q1);
			sw.Restart();
			var q2 = QueryFactory.GetOrAdd<Product>(q => q.Bool(x => x.Filter(y => y.Term(p => p.Name, "321"))));
			sw.Stop();
			Console.WriteLine(sw.ElapsedTicks);
			Console.WriteLine(q2);
			sw.Restart();

			for (var i = 0; i < 10000; i++)
			{
				var q3 = QueryFactory.GetOrAdd<Product>(q => q.Bool(x => x.Filter(y => y.Term(p => p.Name, "111"))));
			}

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			//Console.WriteLine(q3);
			Console.WriteLine(QueryFactory.Count);
		}

		[TestMethod]
		public void QueryTest3()
		{
			var sw = new Stopwatch();
			sw.Start();
			//var s = ElasticQueryBuilder.QueryFactory.GetOrAdd<SearchCommand<Product>>(x => x.Query(q => q.Bool(b => b.Term(p => p.FullName, new FullName(){Category="category", Name="name", Producer = "producer"}))));
			var s = ElasticQueryBuilder.QueryFactory.GetOrAdd<SearchCommand<Product>>(x => x.Query(q => q.Bool(b => b.Term(p => p.Name, "123"))));
			sw.Stop();
			Console.WriteLine(s);
			Console.WriteLine(sw.ElapsedTicks);

			sw.Restart();
			string q3 = null;
			for (var i = 0; i < 10000; i++)
			{
				//q3 = ElasticQueryBuilder.QueryFactory.GetOrAdd<SearchCommand<Product>>(x => x.Query(q => q.Bool(b => b.Term(p => p.FullName, new FullName() { Category = "category", Name = "name", Producer = "producer" }))));
				q3 = ElasticQueryBuilder.QueryFactory.GetOrAdd<SearchCommand<Product>>(x => x.Query(q => q.Bool(b => b.Term(p => p.Name, Guid.NewGuid().ToString()))));
			}

			sw.Stop();
			Console.WriteLine(q3);
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine(sw.ElapsedTicks);
			Console.WriteLine(ElasticQueryBuilder.QueryFactory.Count);
		}

		[TestMethod]
		public void TestDeseriaize()
		{
			var sw = new Stopwatch();
			sw.Start();
			for (var i = 0; i < 1000000; i++)
			{
				var n = new DeserializeProjection("sdgkhdsg", 23);
			}
			sw.Stop();
			Debug.WriteLine(sw.ElapsedMilliseconds);

			var type = typeof(DeserializeProjection);
			var constructor = type.GetConstructors().First();
			var j = 0;
			var parameters = new[] { Expression.Parameter(typeof(string), "name"), Expression.Parameter(typeof(int), "value") };
			var parameters2 = constructor.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name.ToLower())).ToArray();
			var newexp = Expression.New(constructor, parameters2.Take(2).ToArray());

			var c = Expression.Lambda<Func<string, int, DeserializeProjection>>(newexp, parameters2).Compile();
			sw.Restart();
			for (var i = 0; i < 1000000; i++)
			{
				//var n = c.DynamicInvoke("sdgdshsdh", 34);
				var n = c("sdgdshsdh", 34);
			}
			sw.Stop();
			Debug.WriteLine(sw.ElapsedMilliseconds);

			var s = "{'name':'dfgkfjdh', 'value': 12}";
			sw.Restart();
			for (var i = 0; i < 1000000; i++)
			{
				using (var reader = new StringReader(s))
				using (var jsonreader = new JsonTextReader(reader))
				{
					string name = null;
					int value = 0;
					while (jsonreader.Read())
					{
						if (jsonreader.TokenType == JsonToken.PropertyName && jsonreader.Value == "name")
							name = jsonreader.ReadAsString();
						if (jsonreader.TokenType == JsonToken.PropertyName && jsonreader.Value == "value")
							value = jsonreader.ReadAsInt32().Value;
					}
					var n = c(name, value);
				}
			}
			sw.Stop();
			Debug.WriteLine(sw.ElapsedMilliseconds);

			sw.Restart();
			for (var i = 0; i < 1000000; i++)
			{
				var n = JsonConvert.DeserializeObject<DeserializeProjection>(s);
			}
			sw.Stop();
			Debug.WriteLine(sw.ElapsedMilliseconds);
		}

		//[TestMethod]
		//public void ESPerfomance()
		//{
		//	for (var i = 0; i < 1000000; i++)
		//	{
		//		_repository.Insert(new CategoryProjection() { CreatedOnUtc = DateTime.Now, Name = Guid.NewGuid().ToString() }, false);
		//	}
		//}
	}
}
