using System;
using Core.ElasticSearch.Domain;
using Nest;
using ToDo.Dal.Interfaces;

namespace ToDo.Dal.Projections
{
	public class Task : BaseEntityWithParent<Models.Project, Models.Project>, IProjection<Models.Task>, IWithUser, IWithVersion
	{
		[Keyword]
		public User User { get; set; }
		public int Version { get; set; }
		public string Name { get; set; }
		public string Note { get; set; }
		public DateTime Created { get; set; }
		[Keyword]
		public Projections.Task ParentTask { get; set; }
		[Keyword]
		public Projections.User Assign { get; set; }
		public int EstimatedTime { get; set; }
		public int ActualTime { get; set; }
		public DateTime Deadline { get; set; }
	}
}