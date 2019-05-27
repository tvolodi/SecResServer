using Microsoft.EntityFrameworkCore;
using SecResServer.Libs;
using SecResServer.Model;
using SecResServer.Model.Edgar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Jobs
{
    public class DownloadEdgarCompanies
    {

        SecResDbContext dbContext;

        public DownloadEdgarCompanies(SecResDbContext secResDbContext)
        {
            dbContext = secResDbContext;
        }



        public void Execute()
        {
            string address = "https://datafied.api.edgar-online.com";
            string apiName = "descriptions";
            string resourceName = "companies";
            string fieldName = "primarysymbol";
            int lastTimeRecordQnt = 0;
            bool isFirstRun = true;
            int recordsLimit = 999;
            int recordsOffset = 0;
            string apiKey = "";

            Task.Run(async () =>
            {
                while (lastTimeRecordQnt > 0 || isFirstRun)
                {
                    string uri = $"{address}/v2/{apiName}/{resourceName}/{fieldName}?limit={recordsLimit}&offset={recordsOffset}&appkey={apiKey}";

                    EdgarDescriptions edgarDescriptions = await HttpReqExec.GetAsync<EdgarDescriptions>(uri);

                    List<string> symbolList = edgarDescriptions.Descriptions;

                    for (int i = 0; i < symbolList.Count; i++)
                    {
                        EdgarCompany edgarCompany = await dbContext.EdgarCompanies.Where(es => es.PrimarySymbol == symbolList[i]).FirstOrDefaultAsync();
                        if (edgarCompany == null)
                        {
                            string symbol = symbolList[i];
                            if (symbol != null && symbol.Length > 0)
                            {
                                edgarCompany = new EdgarCompany { PrimarySymbol = symbolList[i] };
                                await dbContext.AddAsync(edgarCompany);
                            }
                        }
                        else
                        {
                            edgarCompany.LastUpdateDT = DateTime.Now;
                        }
                    }
                    await dbContext.SaveChangesAsync();

                    recordsOffset += recordsLimit;
                    lastTimeRecordQnt = symbolList.Count;
                    isFirstRun = false;

                }
            }).Wait();

        }
    }
}
