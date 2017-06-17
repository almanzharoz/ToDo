using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ToDo.WebApp.Policies
{
	public class UserHostRequirement : IAuthorizationRequirement
	{
	}
	public class UserHostHandler : AuthorizationHandler<UserHostRequirement>
	{
		private readonly IHttpContextAccessor _httpAccessor;
		public UserHostHandler(IHttpContextAccessor httpAccessor)
		{
			_httpAccessor = httpAccessor;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserHostRequirement requirement)
		{
			string claimValue = context.User.FindFirst(c => c.Type == "IP")?.Value;

			if (!string.IsNullOrEmpty(claimValue) && claimValue == _httpAccessor.HttpContext.Request.Host.Host)
			{
				context.Succeed(requirement);
			}
			else
			{
				context.Fail();
			}
			return Task.CompletedTask;
		}
	}
}