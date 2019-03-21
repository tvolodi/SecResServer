using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SecResServer.Workflows
{
    public class LoadStockPriceWF : IWorkflow
    {
        public string Id => "LoadStockPriceWF";

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder.
                StartWith<DownloadNasdaqStockListStep>()
                .Then<ImportNasdaqStockListStep>();
        }
    }
}
