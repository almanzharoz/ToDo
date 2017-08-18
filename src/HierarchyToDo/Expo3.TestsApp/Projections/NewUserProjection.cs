using Core.ElasticSearch.Domain;
using Expo3.Model.Models;

namespace Expo3.TestsApp.Projections
{
	public class NewUserProjection : BaseEntity, IProjection<User>, IInsertProjection
	{
		public string Name { get; set; }
	}
}