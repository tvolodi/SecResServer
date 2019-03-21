using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SecResServer.Workflows
{
    public class ImportNasdaqStockListStep : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            try
            {

                return ExecutionResult.Next();
            } catch (Exception e)
            {
                return ExecutionResult.Next();
            }
        }
    }
}
