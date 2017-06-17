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
	public class ProjectController : BaseController<ProjectRepository>
	{
		private readonly ILogger<ProjectController> _logger;
		public ProjectController(ILoggerFactory loggerFactory, ProjectRepository repository) : base(repository)
		{
			_logger = loggerFactory.CreateLogger<ProjectController>();
		}

		[HttpGet]
		public IActionResult Add() => View(new ProjectEditModel());

		[HttpGet]
		public IActionResult Edit(string id) => View(new ProjectEditModel(_repository.GetProject(id)));

		[HttpPost]
		public IActionResult Edit(ProjectEditModel project)
			=> ModelState.IsValid
				? View("Saved", _repository.SaveProject(project.Id, project.Update))
				: View(project);

		[HttpDelete]
		public IActionResult Delete(string id, int version) =>
			View(id.HasNotNullArg(nameof(id))
				.Try(
					x => _repository.Delete(x, version.HasNotNullArg(nameof(version))),
					(x, e) =>
					{
						_logger.LogCritical($"Project (id: {x}) delete error: {e}");
						return false;
					}));

		public IActionResult Index() => View(_repository.GetMyProjectsWithCount());

		[HttpGet]
		public IActionResult AddUser(string id) => View("AddUser", id);
		[HttpPost]
		public IActionResult AddUser(string id, string user) => View("UserAdding", _repository.AddUser(id, user));
		[HttpDelete]
		public IActionResult DeleteUser(string id, string user) => View(_repository.DeleteUser(id, user));

		public IActionResult GetUsers(string id, string s) =>
			Json(_repository.GetUsersNames(id, s).Select(x => new {x.Id, Name = x.Nick}));

	}
}