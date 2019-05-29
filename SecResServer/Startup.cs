using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecResServer.Hangfire;
using Hangfire.PostgreSql;
using Hangfire;
using SecResServer.Hubs;
using SecResServer.Model;
using SecResServer.Jobs;

namespace SecResServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<HfDbContext>(options => 
            //    options.UseNpgsql(Configuration.GetConnectionString("HfDbConnection")));

            services.AddHangfire(configuration => configuration.UsePostgreSqlStorage(Configuration.GetConnectionString("HfDbConnection")));

            services.AddSignalR();

            services.AddMvc();

            services.AddDbContext<SecResDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("SecResDbConnection"));
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<SecHub>("/secHub");
            });

            SimFinDataDownloader simFinDataDownloader = new SimFinDataDownloader(Configuration.GetConnectionString("SecResDbConnection"));
            Task.Run(async () =>
            {
                await simFinDataDownloader.DownloadAllStmtAsync();
            });


            app.Run(async (context) =>
            {

                //await context.Response.WriteAsync("Hello World!");
                
                //SimFinDataDownloader simFinDataDownloader = new SimFinDataDownloader(Configuration.GetConnectionString("SecResDbConnection"));
                //await simFinDataDownloader.DownloadAllEntitiesAsync();
                
            });


        }
    }
}
