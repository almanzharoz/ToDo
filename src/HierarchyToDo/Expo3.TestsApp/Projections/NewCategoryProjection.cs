using Core.ElasticSearch.Domain;
using Expo3.Model.Models;

namespace Expo3.TestsApp.Projections
{
	internal class NewCategoryProjection : BaseEntity, IProjection<Category>, IInsertProjection
	{
		public string Name { get; set; }
	}
}