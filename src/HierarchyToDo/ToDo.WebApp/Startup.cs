using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDo.Dal;
using ToDo.Dal.Models;
using ToDo.Dal.Projections;
using ToDo.WebApp.Policies;

namespace ToDo.WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
            {
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
			});
			//services.AddAntiforgery(opts => opts.HeaderName = "X-XSRF-Token"); // http://www.dotnetcurry.com/aspnet/1343/aspnet-core-csrf-antiforgery-token
			services.AddSession();
	        services.AddDistributedMemoryCache();

	        services.AddAuthorization(options =>
	        {
		        options.AddPolicy("ip-policy", policy => policy.Requirements.Add(new UserHostRequirement()));
		        //options.AddPolicy("resource-allow-policy", x => { x.AddRequirements(new PermissionRequirement(new[] { Operations.Read })); });
	        });

	        services.AddScoped<UserName>(x =>
	        {
		        var user = x.GetService<IHttpContextAccessor>()?.HttpContext?.User;
		        if (user != null && user.Identity != null && !String.IsNullOrWhiteSpace(user.FindFirst(ClaimTypes.NameIdentifier)?.Value))
			        return new UserName(user.FindFirst(ClaimTypes.NameIdentifier).Value);
		        return null;
	        });

	        services.AddToDo("http://localhost:9200", "todo");
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

	        app.UseSession();

	        app.UseCookieAuthentication(new CookieAuthenticationOptions()
	        {
		        AuthenticationScheme = "MyCookieMiddlewareInstance",
		        CookieName = "MyCookieMiddlewareInstance",
		        LoginPath = new PathString("/Account/Login"),
		        AccessDeniedPath = new PathString("/Account/AccessDenied"),
		        AutomaticAuthenticate = true,
		        AutomaticChallenge = true
	        });

			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

	        app.UseToDo();
        }
    }
}
