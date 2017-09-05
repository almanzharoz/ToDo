using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeeFee.ImagesWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
	        services.Configure<ImagesWebSettings>(Configuration.GetSection("Settings"));
	        services.AddSingleton(cfg => cfg.GetService<IOptions<ImagesWebSettings>>().Value);

			services.AddMvc();

	        services.AddScoped(x => new ImageService(x.GetService<IOptions<ImagesWebSettings>>().Value.ImagesFolder));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

	        app.UseStaticFiles(Configuration["PublicImagesFolder"]);

            app.UseMvc();
        }
    }
}
