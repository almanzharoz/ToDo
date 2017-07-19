using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharpFuncExt;
using ToDo.Dal;
using ToDo.Dal.Repositories;
using ToDo.WebApp.Model;

namespace ToDo.WebApp.Controllers
{
	[Authorize(Roles = "manager")]
	public class ProjectController : BaseController<ProjectService>
	{
		private readonly ILogger<ProjectController> _logger;
		public ProjectController(ILoggerFactory loggerFactory, ProjectService service) : base(service)
		{
			_logger = loggerFactory.CreateLogger<ProjectController>();
		}

		[HttpGet]
		public IActionResult Add() => View(new ProjectEditModel());

		[HttpGet]
		public IActionResult Edit(string id) => View(new ProjectEditModel(_service.GetProject(id)));

		[HttpPost]
		public IActionResult Edit(ProjectEditModel project)
			=> ModelState.If(x => project.Id != null ? _service.ProjectExists(project.Id, project.Name) : _service.ProjectExists(project.Name), x=>x.AddModelError("Name", "Project already exists"))
			.IsValid
				? View("Saved", _service.SaveProject(project.Id, p => p.Set(x => x.Name, project.Name)))
				: View(project);

		[HttpDelete]
		public IActionResult Delete(string id, int version) =>
			View(id.HasNotNullArg(nameof(id))
				.Try(
					x => _service.Delete(x, version.HasNotNullArg(nameof(version))),
					(x, e) =>
					{
						_logger.LogCritical($"Project (id: {x}) delete error: {e}");
						return false;
					}));

		public IActionResult Index() => View(_service.GetMyProjectsWithCount());

		[HttpGet]
		public IActionResult AddUser(string id) => View("AddUser", id);
		[HttpPost]
		public IActionResult AddUser(string id, string user) => View("UserAdding", _service.AddUser(id, user));
		[HttpDelete]
		public IActionResult DeleteUser(string id, string user) => View(_service.DeleteUser(id, user));

		public IActionResult GetUsers(string id, string s) =>
			Json(_service.GetUsersNames(id, s).Select(x => new {x.Id, Name = x.Nick}));

	}
}