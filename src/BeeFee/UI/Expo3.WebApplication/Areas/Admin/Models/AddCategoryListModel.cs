using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Expo3.AdminApp.Projections;
using Expo3.Model.Models;

namespace Expo3.WebApplication.Areas.Admin.Models
{
	public class AddCategoryListModel
	{
		public IReadOnlyCollection<CategoryProjection> ExistingCategories { get; }
		[Required]
		public string Name { get; set; }
		public string Url { get; set; }

		public AddCategoryListModel()
		{
		}

		public AddCategoryListModel(IReadOnlyCollection<CategoryProjection> existingCategories)
		{
			ExistingCategories = existingCategories;
		}
	}
}