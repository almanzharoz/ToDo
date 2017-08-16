using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Expo3.LoginApp.Services;
using Expo3.Model.Embed;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Expo3.WebApplication.Models;

namespace Expo3.WebApplication.Controllers
{
	public class AccountController : BaseController<AuthorizationService>
	{
		public AccountController(AuthorizationService service) : base(service)
		{
		}

		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			return View(new LoginViewModel() { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel vm)
		{
			var user = _service.TryLogin(vm.Login.Trim().ToLower(), vm.Pass.Trim());
			if (user == null)
				return View(vm);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim("IP", Request.Host.Host, ClaimValueTypes.String),
				new Claim("permission-foo", "grant")
			};
			claims.AddRange((user.Roles ?? new[] { EUserRole.Anonym }).Select(x => new Claim(ClaimTypes.Role, x.ToString().ToLower())));

			var identity = new ClaimsIdentity("MyCookieMiddlewareInstance");
			identity.AddClaims(claims);

			var principal = new ClaimsPrincipal(identity);

			await HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance",
				principal,
				new AuthenticationProperties
				{
					ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
				});

			//_logger.LogInformation(4, "User logged in.");

			return Redirect(vm.ReturnUrl ?? "/");
		}

		[HttpGet]
		public async Task<IActionResult> LogOff()
		{
			await HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");

			//_logger.LogInformation(4, "User logged out.");

			return Redirect("/");
		}

		public IActionResult AccessDenied()
		{
			return Content("Access Denied");
		}

		public IActionResult Register() // TODO: добавить регистрацию
		{
			_service.Register("admin@mail.ru", "admin", "123", new[] {EUserRole.Organizer, EUserRole.User});
			return Content("Done");
		}
	}
}