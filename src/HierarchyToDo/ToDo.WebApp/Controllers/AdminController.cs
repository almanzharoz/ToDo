﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using ToDo.Dal;
using ToDo.Dal.Repositories;
using ToDo.WebApp.Model.Admin;

namespace ToDo.WebApp.Controllers
{
	[Authorize(Roles = "admin")]
    public class AdminController : BaseController<AdminService>
    {
	    public AdminController(AdminService service) : base(service)
	    {
	    }

		[HttpGet]
		public IActionResult AddUser() => PartialIfAjax(new NewUserEditModel(){Roles = "user"}, x => View(x));
		[HttpPost]
		public IActionResult AddUser(NewUserEditModel model)
		    => ModelState.IsValid ?
			View("UserAdded", _service.AddUser(model.Email, model.Name, model.Password, model.Roles.Split('\n').Select(x => x.Trim()).Where(StringFunc.IsNotNull).ToArray()))
			: View(model);

	    public IActionResult Users()
		    => View(_service.GetUsers());
		[HttpPost]
	    public IActionResult Deny(string id, bool deny)
		    => View(deny.Extend(_service.DenyUser(id.HasNotNullArg(nameof(id)), deny)));
		[HttpDelete]
	    public IActionResult DeleteRole(string id, string role)
		    => View(_service.DeleteRole(id.HasNotNullArg(nameof(id)), role.HasNotNullArg(nameof(role))));

	    [HttpGet]
	    public IActionResult AddRole(string id) => PartialIfAjax(id, "AddRole", x => View("AddRole", x));
		[HttpPost]
	    public IActionResult AddRole(string id, string role)
		    => View("RoleAdded", _service.AddRole(id.HasNotNullArg(nameof(id)), role.HasNotNullArg(nameof(role))));
		[HttpDelete]
		public IActionResult Delete(string id)
		    => View(_service.DeleteUser(id.HasNotNullArg(nameof(id))));
	}
}
