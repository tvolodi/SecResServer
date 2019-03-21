using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SecResServer.Workflows
{
    public class DownloadNasdaqStockListStep : StepBodyAsync
    {
        public string DownloadFileUrl { get; set; }

        public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            try
            {
                WebClient wc = new WebClient();
                Uri fileUri = new Uri("https://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nasdaq&render=download");
                await wc.DownloadFileTaskAsync(fileUri, "nasdaq.csv");

                // If all OK then advance to the next step
                return ExecutionResult.Next();
            }
            catch (Exception e)
            {
                // Try to repeat the step after a while
                // TODO: break a cycle after several tries
                return ExecutionResult.Sleep(TimeSpan.FromSeconds(60), new object());
            }
        }
    }
}
