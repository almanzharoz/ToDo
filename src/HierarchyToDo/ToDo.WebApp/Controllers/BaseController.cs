using System;
using System.Diagnostics;
using System.Linq;
using Core.ElasticSearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDo.Dal.Services;

namespace ToDo.WebApp.Controllers
{
	public abstract class BaseController<TService> : Controller 
		where TService : BaseToDoService
	{
		protected readonly TService _service;

		protected BaseController(TService service)
		{
			_service = service;
		}

		protected bool IsAjax => Request.Headers.ContainsKey("X-Requested-With") &&
								Request.Headers["X-Requested-With"] == "XMLHttpRequest";

		protected bool IsJson => Request.Headers.ContainsKey("AcceptTypes") &&
								Request.Headers["AcceptTypes"].Any(x => x.ToLower().IndexOf("json", StringComparison.Ordinal) != -1);

		protected T IfAjax<T>(Func<T> ifTrue, Func<T> ifFalse) => IsAjax ? ifTrue() : ifFalse();
		protected T IfAjax<T, TArg>(TArg arg, Func<TArg, T> ifTrue, Func<TArg, T> ifFalse) => IsAjax ? ifTrue(arg) : ifFalse(arg);
		protected IActionResult PartialIfAjax<TArg, T>(TArg arg, Func<TArg, T> ifFalse) where T : IActionResult
			=> IsAjax ? (IActionResult)PartialView(arg) : ifFalse(arg);

		protected IActionResult PartialIfAjax<TArg, T>(TArg arg, string viewName, Func<TArg, T> ifFalse) where T : IActionResult
			=> IsAjax ? (IActionResult)PartialView(viewName, arg) : ifFalse(arg);

		protected IActionResult PartialIfAjax<TArg, T>(Func<TArg> getarg, Func<TArg, T> ifFalse) where T : IActionResult
		{
			var arg = getarg();
			return IsAjax ? (IActionResult) PartialView(arg) : ifFalse(arg);
		}

		protected T IfJson<T>(Func<T> ifTrue, Func<T> ifFalse) => IsJson ? ifTrue() : ifFalse();

		[NonAction]
		public override ViewResult View(string viewName, object model)
		{
			if (IsAjax)
				ViewBag.Layout = "_PartialLayout";
			return base.View(viewName, model);
		}

		private Stopwatch sw = new Stopwatch();
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			sw.Start();
			base.OnActionExecuting(context);
		}

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			base.OnActionExecuted(context);
			sw.Stop();
			Debug.WriteLine("Action: "+sw.ElapsedMilliseconds);
		}
	}

	public static class ControllerExtensions
	{
		
	}
}