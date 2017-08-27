using Expo3.AdminApp.Services;
using Expo3.WebApplication.Areas.Admin.Models;
using Expo3.WebApplication.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo3.WebApplication.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "admin")]
	public class UsersController : BaseController<UserService>
	{
		public UsersController(UserService service) : base(service)
		{
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Add()
		{
			return PartialView(new AddUserModel());
		}

		[HttpPost]
		public IActionResult Add(AddUserModel addUserModel)
		{
			_service.AddUser(
				addUserModel.Email,
				addUserModel.Password,
				addUserModel.Name,
				addUserModel.Roles);

			return RedirectToAction("Index");
		}
	}
}
