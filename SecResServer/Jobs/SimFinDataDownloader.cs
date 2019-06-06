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
            
            int simFinEntityId = 0;
                 
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                List<SimFinEntity> simFinEntities = await dbContext.SimFinEntities.ToListAsync();
                for (int i = 0; i < simFinEntities.Count; i++)
                {
                    SimFinEntity entity = simFinEntities[i];
                    simFinEntityId = entity.SimFinId;
                    string httpReqString = $"{baseAddress}{serviceName}/{simFinEntityId}/{subServiceName}?api-key={apiKey}";

                    JObject simFinStmtRegistry = await Libs.SimFinHttpReqExec.ExecSimFinHttpReqAsync<JObject>(httpReqString, dbConnectionString);

                    await RegisterStmtAsync("pl", simFinStmtRegistry, simFinEntityId);
                    await RegisterStmtAsync("bs", simFinStmtRegistry, simFinEntityId);
                    await RegisterStmtAsync("cf", simFinStmtRegistry, simFinEntityId);
                }
            }

            totalQnt = 1;
            return totalQnt;
        }

        private async Task RegisterStmtAsync(string stmtType, JObject jObject, int simFinEntityIdCode)
        {
            int simFinEntityId = 0;
            JToken stmtTypeJson = jObject[stmtType];
            List<StmtEntity> stmts = new List<StmtEntity>();
            using(SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                foreach (var item in stmtTypeJson)
                {
                    StmtEntity stmtEntity = JsonConvert.DeserializeObject<StmtEntity>(item.ToString());

                    SimFinStmtRegistry stmt = await dbContext
                                                    .SimFinStmtRegistries
                                                    .Include(sr => sr.StmtType)
                                                    .Include(sr => sr.PeriodType)
                                                    .Where(sr => sr.PeriodType.Name == stmtEntity.Period
                                                                        && sr.FYear == stmtEntity.FYear
                                                                        && sr.StmtType.Name == stmtType)
                                                    .FirstOrDefaultAsync();
                    if (stmt == null)
                    {
                        int periodId = await dbContext.PeriodTypes.Where(pt => pt.Name == stmtEntity.Period).Select(pt => pt.Id).FirstOrDefaultAsync();
                        if(periodId == 0)
                        {
                            PeriodType periodType = new PeriodType
                            {
                                Name = stmtEntity.Period
                            };
                            await dbContext.AddAsync(periodType);
                            await dbContext.SaveChangesAsync();
                            periodId = periodType.Id;
                        }
                        int stmtTypeId = await dbContext.StmtTypes.Where(st => st.Name == stmtType).Select(st => st.Id).FirstOrDefaultAsync();
                        if(stmtTypeId == 0)
                        {
                            StmtType type = new StmtType
                            {
                                Name = stmtType
                            };
                            await dbContext.AddAsync(type);
                            await dbContext.SaveChangesAsync();
                            stmtTypeId = type.Id;
                        }

                        if(simFinEntityId == 0)
                        {
                            simFinEntityId = await dbContext.SimFinEntities
                                                                .Where(e => e.SimFinId == simFinEntityIdCode)
                                                                .Select(e => e.Id)
                                                                .FirstOrDefaultAsync();
                        }

                        stmt = new SimFinStmtRegistry
                        {
                            IsCalculated = stmtEntity.IsCalculated,
                            PeriodTypeId = periodId,
                            SimFinEntityId = simFinEntityId,
                            StmtTypeId = stmtTypeId,
                            LoadDateTime = DateTime.Now,
                            IsStmtLoaded = false,
                            FYear = stmtEntity.FYear
                        };

                        try
                        {
                            await dbContext.AddAsync(stmt);
                        } catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        try
                        {
                            await dbContext.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        // Load original and standardized statements
                        
                    }
                }

            }


        }
    }
}
