using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecResServer.Model;
using SecResServer.Model.SimFin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Jobs
{
    public class SimFinDataDownloader 
    {

        private SecResDbContext dbContext;
        private string dbConnectionString = string.Empty;
        private string baseAddress = "https://simfin.com/api/v1/";


        string apiKey = "6sFg1i77TDYXdtBfRb1Ga93lD4XGTuZZ";

        public SimFinDataDownloader(string dbConnectionString)
        {

            this.dbConnectionString = dbConnectionString;
            
        }

        public void ExecuteDaily()
        {
            // string baseAddress = "https://simfin.com/api/v1/info/";
            
            //string apiName = "descriptions";
            //string resourceName = "companies";
            //string fieldName = "primarysymbol";
            //int lastTimeRecordQnt = 0;
            //bool isFirstRun = true;
            //int recordsLimit = 999;
            //int recordsOffset = 0;
            

            Task.Run(async () =>
            {
                await DownloadAllEntitiesAsync();
            });
        }

        public async Task<int> DownloadAllEntitiesAsync()
        {
            int totalQnt = 0;
            string serviceName = "info/all-entities";
            string httpReqString = $"{baseAddress}{serviceName}?api-key={apiKey}";
            List<SimFinEntity> simFinEntities = await Libs.HttpReqExec.GetAsync<List<SimFinEntity>>(httpReqString);
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                for (int i = 0; i < simFinEntities.Count; i++)
                {
                    SimFinEntity currSimFinEntity = simFinEntities[i];

                    SimFinEntity simFinEntity = await dbContext.SimFinEntities.Where(se => se.Ticker == currSimFinEntity.Ticker).FirstOrDefaultAsync();
                    if(simFinEntity == null)
                    {
                        
                        simFinEntity = new SimFinEntity
                        {
                            Name = currSimFinEntity.Name,
                            SimFinId = currSimFinEntity.SimFinId,
                            Ticker = currSimFinEntity.Ticker
                        };
                        await dbContext.AddAsync(simFinEntity);
                    }
                    else
                    {
                        simFinEntity.LastUpdateDT = DateTime.Now;
                        dbContext.Entry(simFinEntity).State = EntityState.Modified;
                    }

                }
                await dbContext.SaveChangesAsync();

            }


            totalQnt = simFinEntities.Count();
            return totalQnt;
        }

        public async Task<int> DownloadAllStmtAsync()
        {
            int totalQnt = 0;
            string serviceName = "companies/id";
            string subServiceName = "statements/list";
            string httpReqString = $"{baseAddress}{serviceName}/111052/{subServiceName}?api-key={apiKey}";
            JObject simFinStmtRegistry = await Libs.HttpReqExec.GetAsync<JObject>(httpReqString);

            await RegisterStmtAsync("pl", simFinStmtRegistry);
            await RegisterStmtAsync("bs", simFinStmtRegistry);
            await RegisterStmtAsync("cf", simFinStmtRegistry);


            // 
            //StmtListJsonConvEntity simFinStmtRegistry = await Libs.HttpReqExec.GetAsync<StmtListJsonConvEntity>(httpReqString);


            //JToken plJson = simFinStmtRegistry["pl"];
            //List<StmtEntity> plStmts = new List<StmtEntity>();

            //foreach(var item in plJson)
            //{
            //    StmtEntity stmtEntity = JsonConvert.DeserializeObject<StmtEntity>(item.ToString());
            //    plStmts.Add(stmtEntity);                
            //}

            //Dictionary<int, StmtEntity> plDict = plJson.ToDictionary(pair => pair.Key, pair => (StmtEntity)pair.Value);
            //JsonConvert.DeserializeObject<Dictionary<int, StmtEntity>>(

            //using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            //{
            //    for (int i = 0; i < simFinEntities.Count; i++)
            //    {
            //        SimFinEntity currSimFinEntity = simFinEntities[i];

            //        SimFinEntity simFinEntity = await dbContext.SimFinEntities.Where(se => se.Ticker == currSimFinEntity.Ticker).FirstOrDefaultAsync();
            //        if (simFinEntity == null)
            //        {

            //            simFinEntity = new SimFinEntity
            //            {
            //                Name = currSimFinEntity.Name,
            //                SimFinId = currSimFinEntity.SimFinId,
            //                Ticker = currSimFinEntity.Ticker
            //            };
            //            await dbContext.AddAsync(simFinEntity);
            //        }
            //        else
            //        {
            //            simFinEntity.LastUpdateDT = DateTime.Now;
            //            dbContext.Entry(simFinEntity).State = EntityState.Modified;
            //        }

            //    }
            //    await dbContext.SaveChangesAsync();

            //}


            totalQnt = 1;
            return totalQnt;
        }

        private async Task RegisterStmtAsync(string stmtType, JObject jObject)
        {
            JToken stmtTypeJson = jObject[stmtType];
            List<StmtEntity> stmts = new List<StmtEntity>();

            foreach (var item in stmtTypeJson)
            {
                StmtEntity stmtEntity = JsonConvert.DeserializeObject<StmtEntity>(item.ToString());

                SimFinStmtRegistry stmt = await dbContext
                                                .simFinStmtRegistries
                                                .Include(sr => sr.StmtType)
                                                .Include(sr => sr.PeriodType)
                                                .Where(sr => sr.PeriodType.Name == stmtEntity.Period
                                                                    && sr.FYear == stmtEntity.FYear
                                                                    && sr.StmtType.Name == stmtType)
                                                .FirstOrDefaultAsync();
            }

        }
    }
}
