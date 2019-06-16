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
            int tryCount = 10;
            bool isCallSuccess = false;
            bool isToExit = false;
            T result;
            Object result2 = null;

            while (isCallSuccess != true || tryCount == 0)
            {
                try
                {
                    using (SecResDbContext dbContext = new SecResDbContext(dbConnectionStr))
                    {
                        while (!isToExit)
                        {
                            int recQnt = await dbContext.SimFinRequestLogs
                                                         .Where(l => l.RequestDT >= DateTime.Now.AddDays(-1))
                                                         .CountAsync();
                            if (recQnt < 2000)
                            {
                                isToExit = true;
                            }
                            else
                            {
                                await Task.Delay(60000);
                            }
                        }
                        result2 = await HttpReqExec.GetAsync<T>(uri);
                        SimFinRequestLog log1 = new SimFinRequestLog
                        {
                            RequestDT = DateTime.Now
                        };
                        await dbContext.AddAsync(log1);
                        await dbContext.SaveChangesAsync();
                        isCallSuccess = true;
                    }

                } catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    tryCount--;
                    if(tryCount == 0) throw e;
                }
 
            }

            result = (T)result2;

            return result;
        }
    }
}
