using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using ToDo.Dal;
using ToDo.Dal.Projections;
using ToDo.Dal.Repositories;
using ToDo.WebApp.Model;

namespace ToDo.WebApp.Controllers
{
	[Authorize(Roles = "user")]
	public class TaskController : BaseController<TaskRepository>
	{
		public TaskController(TaskRepository repository) : base(repository)
		{
		}

		public IActionResult Projects() => View(_repository.GetProjects());

		public IActionResult Index(string id, string parentTaskId) =>
			View((_repository.GetProject(id.HasNotNullArg("projectId")).HasNotNullArg("project"), parentTaskId.IfNotNullOrDefault(_repository.GetTask), _repository.GetTasks(id, parentTaskId).Select(x => new TaskViewModel(x, _repository.GetChildrenCount(x)))));

		public IActionResult MyTasks(string id) =>
			View(_repository.GetMyTasks(id));

		public IActionResult GetTask(string id)
			=> View(_repository.GetTask(id));

		[HttpGet]
		public IActionResult AddTask(string id, string parentTaskId) =>
			View(new NewTaskEditModel(id, parentTaskId.IfNotNullOrDefault(_repository.GetTask)));

		[HttpPost]
		public IActionResult AddTask(NewTaskEditModel model) =>
			ModelState.IsValid.If(
				() => View("TaskAdded",
					_repository.AddTask(
						model.ProjectId,
						model.ParentTaskId,
						model.Name,
						model.Note,
						model.Deadline,
						model.EstimatedTime,
						model.Assigned.IfNotNullOrDefault(x => new UserName(x)))),
				() => View(model.Init(model.ParentTaskId.IfNotNullOrDefault(_repository.GetTask))));

		public IActionResult GetUsers(string id, string s) =>
			Json(_repository.GetUsersNames(id, s).Select(x => new { x.Id, Name = x.Nick }));

	}
}