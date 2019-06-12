using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecResServer.Model;
using SecResServer.Model.SimFin;
using System;
using System.Collections.Generic;
using System.Globalization;
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

                    string[] stmtTypes = new string[] { "pl", "bs", "cf" };
                    for(int itemCnt = 0; itemCnt < stmtTypes.Length; itemCnt++)
                    {
                        await RegisterStmtAsync(stmtTypes[itemCnt], simFinStmtRegistry, simFinEntityId);
                    }
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
                // Cycle through all statements of given type and register them in DB
                foreach (var item in stmtTypeJson)
                {
                    // Get a statement item
                    StmtEntity stmtEntity = JsonConvert.DeserializeObject<StmtEntity>(item.ToString());

                    // Search DB for the statement
                    SimFinStmtRegistry stmt = await dbContext
                                                    .SimFinStmtRegistries
                                                    .Include(sr => sr.StmtType)
                                                    .Include(sr => sr.PeriodType)
                                                    .Where(sr => sr.PeriodType.Name == stmtEntity.Period
                                                                        && sr.FYear == stmtEntity.FYear
                                                                        && sr.StmtType.Name == stmtType)
                                                    .FirstOrDefaultAsync();

                    // If a statement entry is not registered in DB then register
                    if (stmt == null)
                    {
                        // Get period type Id
                        int periodId = await dbContext.PeriodTypes.Where(pt => pt.Name == stmtEntity.Period).Select(pt => pt.Id).FirstOrDefaultAsync();

                        // If the period type is not registered in DB then register it
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

                        // Get statement type Id
                        int stmtTypeId = await dbContext.StmtTypes.Where(st => st.Name == stmtType).Select(st => st.Id).FirstOrDefaultAsync();

                        // If the statement type is not registered then register in DB
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

                        // Get reporting entity DB Id
                        if(simFinEntityId == 0)
                        {
                            simFinEntityId = await dbContext.SimFinEntities
                                                                .Where(e => e.SimFinId == simFinEntityIdCode)
                                                                .Select(e => e.Id)
                                                                .FirstOrDefaultAsync();
                        }

                        // Create an statement instance
                        stmt = new SimFinStmtRegistry
                        {
                            IsCalculated = stmtEntity.IsCalculated,
                            PeriodTypeId = periodId,
                            SimFinEntityId = simFinEntityId,
                            StmtTypeId = stmtTypeId,
                            LoadDateTime = DateTime.Now,
                            IsStmtLoaded = false,
                            FYear = stmtEntity.FYear,
                            
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


                    }

                    // Load original statement

                    await LoadOriginalStmtAsync(stmt, stmtType);

                    // Load standardized statements                      
                }
            }
        }

        private async Task LoadOriginalStmtAsync(SimFinStmtRegistry stmt, string stmtType)
        {
            // Check that the statement is not loaded already
            bool isToLoad = false;
            SimFinOriginalStmt simFinOriginalStmt = null;
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                // Get original statement for this type of statement
                simFinOriginalStmt = await dbContext.SimFinOriginalStmts
                                                    .Where(os => os.IsStmtDetailsLoaded == false
                                                                && os.SimFinStmtRegistryId == stmt.Id)
                                                    .FirstOrDefaultAsync();

                // If the original statement is not created then load from the site and create
                if (simFinOriginalStmt == null)
                {
                    string serviceName = "companies/id";
                    string subServiceName = "statements/original";

                    string httpReqString = $"{baseAddress}{serviceName}/{stmt.SimFinEntityId}/{subServiceName}?stype={stmtType}&ptype={stmt.PeriodType.Name}&fyear={stmt.FYear}&api-key={apiKey}";

                    // Get and parse json for the original statement
                    JObject simFinStmtDetails = await Libs.SimFinHttpReqExec.ExecSimFinHttpReqAsync<JObject>(httpReqString, dbConnectionString);

                    string periodEndDateStr = simFinStmtDetails["periodEndDate"].ToString();
                    DateTime periodEndDate = DateTime.ParseExact(periodEndDateStr, "YYYY-MM-DD", CultureInfo.InvariantCulture);
                    List<JObject> metaTokens = simFinStmtDetails["metaData"].Children<JObject>().ToList();

                    // Don't understand when several meta can be used. Trying to catch this case.
                    if (metaTokens.Count > 1)
                    {
                        throw new Exception("Found more then 1 meta. Exit");
                    }

                    SimFinOriginalStmt origStmt = null;
                    // !!!! The last will be saved only
                    foreach (JObject metaDataJObject in metaTokens)
                    {
                        string firstPublished = metaDataJObject["firstPublished"].ToString();
                        DateTime firstPublishedDate = DateTime.ParseExact(firstPublished, "YYYY-MM-DD", CultureInfo.InvariantCulture);
                        string fYear = metaDataJObject["fyear"].ToString();
                        int fYearInt = int.Parse(fYear);
                        string currencyStr = metaDataJObject["currency"].ToString();
                        string periodTypeStr = metaDataJObject["period"].ToString();

                        int currencyId = await dbContext.Currencies.Where(c => c.CharCode == currencyStr).Select(c => c.Id).FirstOrDefaultAsync();

                        int periodTypeId = await dbContext.PeriodTypes.Where(p => p.Name == periodTypeStr).Select(c => c.Id).FirstOrDefaultAsync();                       

                        origStmt = new SimFinOriginalStmt
                        {
                            CurrencyId = currencyId,
                            FirstPublishedDate = firstPublishedDate,
                            FYear = fYearInt,
                            IsStmtDetailsLoaded = false,
                            PeriodEndDate = periodEndDate,
                            PeriodTypeId = periodTypeId,
                            SimFinStmtRegistryId = stmt.Id
                        };
                        await dbContext.AddAsync(origStmt);
                        await dbContext.SaveChangesAsync();
                    }

                    // Fill statement details? create or update
                    List<JObject> stmtRows = simFinStmtDetails["values"].Children<JObject>().ToList();
                    foreach (JObject rowDetails in stmtRows)
                    {
                        string lineIdStr = rowDetails["lineItemId"].ToString();
                        int lineId = int.Parse(lineIdStr);
                        string lineItemNameStr = rowDetails["lineItemName"].ToString();
                        string valueStr = rowDetails["value"].ToString();
                        double value = double.Parse(valueStr);

                        // Search for the statement details in DB
                        SimFinOrigStmtDetail simFinOrigStmtDetail = await dbContext.SimFinOrigStmtDetails
                                                                                    .Where(osd => osd.LineItemId == lineId
                                                                                                    && osd.SimFinOriginalStmtId == origStmt.Id)
                                                                                    .FirstOrDefaultAsync();

                        if (simFinOrigStmtDetail == null)
                        {
                            StmtDetailName detailName = await dbContext.StmtDetailNames
                                                                        .Where(dn => dn.Name == lineItemNameStr)
                                                                        .FirstOrDefaultAsync();
                            if (detailName == null)
                            {
                                detailName = new StmtDetailName
                                {
                                    Name = lineItemNameStr.Trim()
                                };
                                await dbContext.AddAsync(detailName);
                                await dbContext.SaveChangesAsync();
                            }

                            simFinOrigStmtDetail = new SimFinOrigStmtDetail
                            {
                                LineItemId = lineId,
                                StmtDetailNameId = detailName.Id,
                                Value = value
                            };

                            await dbContext.AddAsync(simFinOrigStmtDetail);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}
