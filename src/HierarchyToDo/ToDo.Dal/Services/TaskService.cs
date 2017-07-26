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
using ToDo.Dal.Services.Internal;
using Task = ToDo.Dal.Projections.Task;

namespace ToDo.Dal.Services
{
	public class TaskService : BaseToDoService
	{
		private readonly UsersService _usersService;

		public TaskService(ILoggerFactory loggerFactory, ElasticConnection settings, ElasticScopeFactory<ElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
			_usersService = factory.GetInternalService<UsersService>();
		}

		public Project GetProject(string id, bool required = false) => _usersService.GetProject(id, required);

		public Task GetTask(string id) =>
			Filter<Models.Task, Task>(q => TaskQuery(q.Ids(x => x.Values(id))))
				.FirstOrDefault();

		public IReadOnlyCollection<Project> GetProjects() =>
			Filter<Project, Project>(q => q.Term(x => x.Users, _user.Id));

		public IReadOnlyCollection<Task> GetTasks(string id, string parentTaskId) =>
			Filter<Models.Task, Task>(q => TaskQuery(
				parentTaskId.IfNotNull(
					x => q.Term(p => p.ParentTask, GetTask(parentTaskId).Id),
					() => !q.Exists(e => e.Field(p => p.ParentTask))), id));

		public bool AddTask(string projectId, string parentTask, string name, string note, DateTime deadline,
			int estimatedTime, UserName assign) =>
			Insert<Task, Project>(new Task
			{
				User = _user,
				Created = DateTime.Now,
				State = ERecordState.New,
				Name = name.HasNotNullArg(nameof(name)),
				States = new[] {new Models.TaskState {Note = note, State = ERecordState.New, Created = DateTime.Now, User = _user}},
				Deadline = deadline,
				EstimatedTime = estimatedTime,
				ParentTask = parentTask.IfNotNullOrDefault(GetTask),
				Assign = assign,
				Parent = GetProject(projectId.HasNotNullArg(nameof(projectId))).HasNotNullArg("project")
			});

		public IReadOnlyCollection<Task> GetMyTasks(string id) =>
			Filter<Models.Task, Task>(q => UserQuery<Task>(null), s => s.Descending(p => p.Created));

		public IEnumerable<Projections.User> GetUsersNames(string id, string s) =>
			Search<Models.User, Projections.User>(q => q.Bool(b => b
					.Must(Query<Models.User>.Match(m => m.Field(p => p.Nick).Query(s)))
					.Filter(GetProject(id.HasNotNullArg("projectId"), true)
						.Users.IfNotNull(u => Query<Models.User>.Ids(f => f.Values(u.Select(x => x.Id))),
							() => new QueryContainer()))),
				sort => sort.Ascending(p => p.Nick), 0, 10);

		public int GetChildrenCount(Task task) =>
			FilterCount<Task>(q => TaskQuery(q.Term(p => p.ParentTask, task.Id), task.Parent.Id));

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
					b.Must(Query<Models.Task>.Match(m => m.Field(f => f.Name).Query(s)) || Query<Models.Task>.Match(m => m.Field(f => f.States.FirstOrDefault().Note).Query(s)))
						.Filter(TaskQuery(Query<Models.Task>.Term(p => p.Assign, _user.Id), id))),
				sort => sort.Ascending(p => p.Deadline));
	}
}