using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using ToDo.Dal.Interfaces;

namespace ToDo.Dal.Models
{
    public class Project : BaseEntityWithVersion, IModel, IProjection<Project>, IWithName, IWithUser, ISearchProjection, IInsertProjection, IUpdateProjection, IRemoveProjection
    {
	    [Keyword]
		public string Name { get; set; }
		[Keyword]
	    public Projections.User User { get; set; }
		[Keyword]
		public Projections.User[] Users { get; set; }
    }
}
