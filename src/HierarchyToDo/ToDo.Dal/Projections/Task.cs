using System;
using Core.ElasticSearch.Domain;
using Nest;
using ToDo.Dal.Interfaces;
using TaskState = ToDo.Dal.Models.TaskState;

namespace ToDo.Dal.Projections
{
	public class Task : BaseEntityWithParent<Models.Project, Models.Project>, IProjection<Models.Task>, IWithUser, IWithVersion
	{
		[Keyword]
		public User User { get; set; }
		public int Version { get; set; }
		public string Name { get; set; }
		public DateTime Created { get; set; }
		[Keyword]
		public Projections.Task ParentTask { get; set; }
		[Keyword]
		public Projections.User Assign { get; set; }
		public int EstimatedTime { get; set; }
		public int ActualTime { get; set; }
		public DateTime Deadline { get; set; }

		public ERecordState State { get; set; }
		public TaskState[] States { get; set; }
	}
}