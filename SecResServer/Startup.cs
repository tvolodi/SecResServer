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
using WorkflowCore.Persistence.PostgreSQL;
using WorkflowCore.Interface;
using SecResServer.Workflows;

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
            services.AddDbContext<HfDbContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("HfDbConnection")));

            services.AddHangfire(configuration => configuration.UsePostgreSqlStorage(Configuration.GetConnectionString("HfDbConnection")));

            services.AddSignalR();

            services.AddWorkflow(cfg => cfg.UsePostgreSQL(Configuration.GetConnectionString("WfConnection"), true, true));
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

            var wfHost = app.ApplicationServices.GetService<IWorkflowHost>();
            wfHost.RegisterWorkflow<LoadStockPriceWF>();
            wfHost.Start();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });


        }
    }
}
