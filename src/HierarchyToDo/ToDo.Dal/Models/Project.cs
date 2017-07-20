using Core.ElasticSearch.Domain;
using Nest;
using Newtonsoft.Json;
using ToDo.Dal.Interfaces;

namespace ToDo.Dal.Models
{
    public class Project : BaseEntity, IProjection<Project>, IWithName, IWithUser, IWithVersion
    {
		[JsonIgnore]
		public int Version { get; set; }
		//[Text(Analyzer = "not_analyzed")]
		public string Name { get; set; }
		[Keyword]
	    public Projections.User User { get; set; }
		[Keyword]
		public Projections.User[] Users { get; set; }
    }
}
