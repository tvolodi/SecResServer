using Microsoft.EntityFrameworkCore;
using SecResServer.Model;
using SecResServer.Model.SimFin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Libs
{
    public class SimFinHttpReqExec
    {
        public async static Task<T> ExecSimFinHttpReqAsync<T>(string uri, string dbConnectionStr)
        {
            bool isToExit = false;
            int callCnt = 0;

            T result;

            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionStr))
            {
                while (!isToExit)
                {
                    int recQnt  = await dbContext.SimFinRequestLogs
                                                 .Where(l => l.RequestDT >= DateTime.Now.AddDays(-1))
                                                 .CountAsync();
                    if (recQnt < 2000)
                    {
                        isToExit = true;
                    } else
                    {
                        await Task.Delay(60000);
                    }
                }
                result = await HttpReqExec.GetAsync<T>(uri);
                SimFinRequestLog log1 = new SimFinRequestLog
                {
                    RequestDT = DateTime.Now
                };
                await dbContext.AddAsync(log1);
                await dbContext.SaveChangesAsync();
            }

            return result;
        }
    }
}
