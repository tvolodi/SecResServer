using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class TestLoadStockPriceWF
    {
        [Fact]
        public void PassingTest()
        {
            IServiceProvider serviceProvider = ConfigueServices();

            var wfHost = serviceProvider.GetService<IWorkflowHost>();
            wfHost.RegisterWorkflow<LoadStockPriceWF>();
            wfHost.Start();
            wfHost.StartWorkflow("LoadStockPriceWF");
        }

        private IServiceProvider ConfigueServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            //services.AddWorkflow();
            services.AddWorkflow(x => x.UsePostgreSQL("Host = localhost; Port = 5433; Username = SecResUser; Password = VolWork1; Database = TestSecResWfServ", true, true));
//            services.AddTransient<GoodbyeWorld>();

            var serviceProvider = services.BuildServiceProvider();

            //config logging
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            // loggerFactory.AddDebug();
            return serviceProvider;
        }
    }
}
