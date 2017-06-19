using System;
using System.Collections.Generic;
using System.Linq;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using SharpFuncExt;
using ToDo.Dal.Interfaces;
using ToDo.Dal.Models;
using ToDo.Dal.Projections;
using Task = ToDo.Dal.Projections.Task;

namespace ToDo.Dal.Repositories
{
	public class TaskRepository : BaseToDoRepository
	{
		public TaskRepository(ILoggerFactory loggerFactory, ElasticSettings settings, ElasticMapping<ElasticSettings> mapping, RequestContainer<ElasticSettings> container, UserName user) 
			: base(loggerFactory, settings, mapping, container, user)
		{
		}

		public Project GetProject(string id, bool required = false) =>
			Search<Project, Project>(
					Query<Project>.Ids(x => x.Values(id.HasNotNullArg("projectId"))) && Query<Project>.Term(x => x.Users, _user.Id))
				.FirstOrDefault().If(required, x => x.HasNotNullArg("project"));

		public Task GetTask(string id) =>
			Search<Models.Task, Task>(TaskQuery(Query<Models.Task>.Ids(x => x.Values(id))))
				.FirstOrDefault();

		public IReadOnlyCollection<Project> GetProjects() =>
			Search<Project, Project>(Query<Project>.Term(x => x.Users, _user.Id));

		public IReadOnlyCollection<Task> GetTasks(string id, string parentTaskId) =>
			Search<Models.Task, Task>(TaskQuery(
				parentTaskId.IfNotNull(
					x => Query<Models.Task>.Term(p => p.ParentTask, GetTask(parentTaskId).Id),
					() => !Query<Models.Task>.Exists(e => e.Field(p => p.ParentTask)))));

		public bool AddTask(string projectId, string parentTask, string name, string note, DateTime deadline, int estimatedTime, UserName assign)
		{
			var t = new Models.Task()
			{
				Parent = GetProject(projectId.HasNotNullArg(nameof(projectId))).HasNotNullArg("project"),
				User = _user,
				Created = DateTime.Now,
				State = ERecordState.New,
				Name = name.HasNotNullArg(nameof(name)),
				Note = note,
				Deadline = deadline,
				EstimatedTime = estimatedTime,
				ParentTask = parentTask.IfNotNullOrDefault(GetTask),
				Assign = assign
			};
			var r = _client.Index(t, x => x.Refresh(Refresh.True).Parent(projectId));
			return true;
		}

		public IReadOnlyCollection<Task> GetMyTasks(string id) =>
			Search<Models.Task, Task>(UserQuery<Task>(null), s => s.Descending(p => p.Created));

		public IEnumerable<Projections.User> GetUsersNames(string id, string s) =>
			Search<Models.User, Projections.User>(q => q.Bool(b => b
					.Must(Query<Models.User>.Match(m => m.Field(p => p.Nick).Query(s)))
					.Filter(GetProject(id.HasNotNullArg("projectId"), true)
						.Users.IfNotNull(u => Query<Models.User>.Ids(f => f.Values(u.Select(x => x.Id))),
							() => new QueryContainer()))),
				sort => sort.Ascending(p => p.Nick), 0, 10);

		public int GetChildrenCount(Task task) =>
			Count<Models.Task>(TaskQuery(Query<Models.Task>.Term(p => p.ParentTask, task.Id), task.Parent.Id));

		private IEnumerable<Task> GetParents(Task task)
		{
			ToDo.Dal.Projections.Task t = task;
			var tasks = new List<ToDo.Dal.Projections.Task>();
			do
			{
				tasks.Add(t);
			} while ((t = t.ParentTask) != null);
			tasks.Reverse();
			return tasks;
		}

		public IEnumerable<Task> GetChildren(IEnumerable<Task> tasks, Task parentTask)
		{
			var result = new List<Task>();
			Task t = null;
			foreach (var task in tasks)
			{
				t = task;
				while ((t = t.ParentTask) != parentTask && t != null) ;
				if (t == parentTask)
					result.Add(task);
			}
			return result;
		}

		public IEnumerable<Task> GetTasksToMe(string id, string s) =>
			Search<Models.Task, Task>(q => q.Bool(b =>
					b.Must(Query<Models.Task>.Match(m => m.Field(f => f.Name).Field(f => f.Note).Query(s)))
						.Filter(TaskQuery(Query<Models.Task>.Term(p => p.Assign, _user.Id), id))),
				sort => sort.Ascending(p => p.Deadline));
	}
}