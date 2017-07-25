using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Dal.Models;
using ToDo.Dal.Services;
using ToDo.Dal.Services.Internal;

namespace ToDo.Dal
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddToDo(this IServiceCollection services, string url)
	    {
		    services.AddElastic(new ElasticConnection(new Uri(url)))
				// внутренние сервисы
				.AddService<UsersService, ElasticConnection>()
				// публичные сервисы
				.AddService<AuthorizationService, ElasticConnection>()
			    .AddService<AdminService, ElasticConnection>()
			    .AddService<ProjectService, ElasticConnection>()
			    .AddService<TaskService, ElasticConnection>();
		    return services;
	    }

	    public static IApplicationBuilder UseToDo(this IApplicationBuilder app) =>
		    app.UseElastic<ElasticConnection, AdminService>(
			    m => m
				    // маппинг
				    .AddMapping<User>(x => x.IndexName)
				    .AddMapping<Project>(x => x.IndexName)
				    .AddMapping<Task>(x => x.IndexName)
					// внутренние документы
					.AddStruct<TaskState>()
				    // проекции
				    .AddProjection<Projections.User, User>()
				    .AddProjection<Projections.UserWithRoles, User>()
				    .AddProjection<Projections.NewUser, User>()
				    .AddProjection<Project, Project>()
				    .AddProjection<Projections.Task, Task, Project>(),
			    rep =>
			    {
				    rep.AddUser("admin", "admin", "123", new[] { EUserRole.Admin });
			    });
    }
}
