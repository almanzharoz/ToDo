using System;
using System.Threading.Tasks;
using Core.ElasticSearch.Domain;
using Core.ElasticSearch.Mapping;
using ToDo.Dal.Interfaces;
using Nest;
using Newtonsoft.Json;

namespace ToDo.Dal.Models
{
	public class Task : BaseEntityWithParent<Project, Project>, IWithName, IWithVersion, IWithCreated, IHasState, IWithUser
	{
		public DateTime Created { get; set; }
		[Keyword]
		public Projections.User User { get; set; }

		[JsonIgnore]
		public int Version { get; set; }
		public ERecordState State { get; set; }
		public TaskState[] States { get; set; }

		[Keyword]
		public Projections.Task ParentTask { get; set; }

		[Keyword]
		public Projections.User Assign { get; set; }
		/// <summary>
		/// Оценочное время, в минутах
		/// </summary>
		public int EstimatedTime { get; set; }
		/// <summary>
		/// Фактически затраченное время, в минутах
		/// </summary>
		public int ActualTime { get; set; }
		public DateTime Deadline { get; set; }

		public string Name { get; set; }
	}
}