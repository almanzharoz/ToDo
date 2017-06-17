using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ToDo.WebApp.Helpers
{
	public static class UrlExtensions
	{
		public static bool IsAjax(this IUrlHelper helper) =>
			helper.ActionContext.HttpContext.Request.Headers.ContainsKey("X-Requested-With") &&
			helper.ActionContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

	}
}