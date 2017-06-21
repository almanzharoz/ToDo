﻿using System;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Dal.Models;
using ToDo.Dal.Repositories;

namespace ToDo.Dal
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddToDo(this IServiceCollection services, string url, string indexName)
	    {
		    services.AddElastic(new ElasticSettings(new Uri(url), indexName))
				// репозитории
				.AddRepository<AuthorizationRepository, ElasticSettings>()
			    .AddRepository<AdminRepository, ElasticSettings>()
			    .AddRepository<ProjectRepository, ElasticSettings>()
			    .AddRepository<TaskRepository, ElasticSettings>();
		    return services;
	    }

	    public static IApplicationBuilder UseToDo(this IApplicationBuilder app) =>
		    app.UseElastic<ElasticSettings, AdminRepository>(
			    m => m
				    // маппинг
				    .AddMapping<User>()
				    .AddMapping<Project>()
				    .AddMapping<Task>()
					
					.AddStruct<TaskState>()
				    // проекции
				    .AddProjection<Projections.User, User>()
				    .AddProjection<Projections.UserWithRoles, User>()
				    .AddProjection<Project, Project>()
				    .AddProjection<Projections.Task, Task, Project, Project>(),
			    rep =>
			    {
				    rep.AddUser("admin", "admin", "123", new[] {"admin"});
			    });
    }
}