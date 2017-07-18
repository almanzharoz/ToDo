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
	public class TaskController : BaseController<TaskService>
	{
		public TaskController(TaskService repository) : base(repository)
		{
		}

		public IActionResult Projects() => View(_service.GetProjects());

		public IActionResult Index(string id, string parentTaskId) =>
			View((_service.GetProject(id.HasNotNullArg("projectId")).HasNotNullArg("project"), parentTaskId.IfNotNullOrDefault(_service.GetTask), _service.GetTasks(id, parentTaskId).Select(x => new TaskViewModel(x, _service.GetChildrenCount(x)))));

		public IActionResult MyTasks(string id) =>
			View(_service.GetMyTasks(id));

		public IActionResult GetTask(string id)
			=> View(_service.GetTask(id));

		[HttpGet]
		public IActionResult AddTask(string id, string parentTaskId) =>
			View(new NewTaskEditModel(id, parentTaskId.IfNotNullOrDefault(_service.GetTask)));

		[HttpPost]
		public IActionResult AddTask(NewTaskEditModel model) =>
			ModelState.IsValid.If(
				() => View("TaskAdded",
					_service.AddTask(
						model.ProjectId,
						model.ParentTaskId,
						model.Name,
						model.Note,
						model.Deadline,
						model.EstimatedTime,
						model.Assigned.IfNotNullOrDefault(x => new UserName(x)))),
				() => View(model.Init(model.ParentTaskId.IfNotNullOrDefault(_service.GetTask))));

		public IActionResult GetUsers(string id, string s) =>
			Json(_service.GetUsersNames(id, s).Select(x => new { x.Id, Name = x.Nick }));

		public IActionResult Board(string id, string parentTaskId, string s) =>
			parentTaskId.IfNotNullOrDefault(_service.GetTask)
				.Convert(parentTask => View((
					s,
					_service.GetProject(id, true),
					parentTask,
					_service.GetTasksToMe(id, s)
						.Convert(x => parentTask.IfNotNull(y => _service.GetChildren(x, y), () => x)))));
	}
}