using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SecResServer.Workflows
{
    public class DownloadNasdaqStockList : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("https://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange=nasdaq&render=download", "nasdaq.csv");

                return ExecutionResult.Next();
            } catch (Exception e)
            {
                return ExecutionResult.Sleep(TimeSpan.FromSeconds(60), new object());
            }
        }
    }
}
